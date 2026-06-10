using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using CommandLine;
using Google.Protobuf.Reflection;

namespace ProtobufGen
{
    static class Program
    {
        public static int Main( string[] args )
        {
            var arguments = Parser.Default.ParseArguments<Options>( args );
            switch ( arguments.Tag )
            {
                case ParserResultType.Parsed:
                    var value = ( ( Parsed<Options> )arguments ).Value;
                    return Run( value );

                case ParserResultType.NotParsed:
                    // The library will automatically write help text. 
                    return -1;

                default:
                    // This should be unreachable.
                    return int.MinValue;
            }
        }

        static int Run( Options arguments )
        {
            var set = ParseFiles( arguments );

            if ( set == null )
            {
                return -1;
            }

            var errors = set.GetErrors();
            PrintErrors( errors, out var numErrors, out var reparse );

            if ( numErrors > 0 )
            {
                return numErrors;
            }
            else if ( reparse )
            {
                set = ReparseFiles( arguments, set );
                errors = set.GetErrors();
                PrintErrors( errors, out numErrors, out reparse );

                Debug.Assert( numErrors == 0, "Errors should have been handled by first pass." );
                Debug.Assert( !reparse, "Should not have to reparse after second pass." );
            }

            var codegen = new SteamKitCSharpCodeGenerator();

            var options = new Dictionary<string, string>
            {
                [ "langver" ] = "11.0",
                [ "names" ] = "original",
                [ "services" ] = "1",
            };

            var files = codegen.Generate( set, options: options );
            var fileName = Path.GetFileName( arguments.ProtobufPath );

            foreach ( var file in files )
            {
                if ( Path.GetFileNameWithoutExtension( file.Name ) != Path.GetFileNameWithoutExtension( fileName ) )
                {
                    continue;
                }

                var dir = Path.GetDirectoryName( arguments.Output );
                if (!string.IsNullOrEmpty(dir))
                {
                    Directory.CreateDirectory( dir );
                }

                var text = file.Text;
                text = text.Replace( "global::ProtoBuf.ProtoContract", "global::LightProto.ProtoContract" );
                text = text.Replace( "global::ProtoBuf.ProtoMember", "global::LightProto.ProtoMember" );
                text = text.Replace( "global::ProtoBuf.IExtensible.GetExtensionObject", "global::LightProto.IExtensible.GetExtensionObject" );
                text = text.Replace( "global::ProtoBuf.IExtension", "global::LightProto.IExtension" );
                text = text.Replace( "global::ProtoBuf.Extensible", "global::LightProto.Extensible" );
                text = text.Replace( "global::ProtoBuf.DataFormat", "global::LightProto.DataFormat" );
                text = text.Replace( "global::System.Collections.Generic.List<byte[]>", "global::System.Collections.Generic.List<global::ProtoBuf.SteamBytes>" );
                text = text.Replace( "public override string ServiceName", $"[global::LightProto.ProtoIgnore]{Environment.NewLine}        public override string ServiceName" );

                text = System.Text.RegularExpressions.Regex.Replace(
                    text,
                    @"public\s+(?<type>[^\s@]+)\s+@(?<name>[a-z]+)\b\s*(?<block>\{[^{}]*\})?(\s*=\s*(?<init>[^;]+);)?",
                    match => {
                        string type = match.Groups["type"].Value;
                        string name = match.Groups["name"].Value;
                        string body = match.Groups["block"].Value;
                        string initValue = match.Groups["init"].Value;

                        string initializer = string.IsNullOrEmpty(initValue) ? "" : $" = {initValue};";

                        if (type.Contains("List<")) {
                            return $"public {type} {name}_ {body}{initializer}\n        [global::LightProto.ProtoIgnore]\n        public {type} @{name} => {name}_;";
                        } else {
                            return $"public {type} {name}_ {body}{initializer}\n        [global::LightProto.ProtoIgnore]\n        public {type} @{name} {{ get => {name}_; set => {name}_ = value; }}";
                        }
                    }
                );

                text = System.Text.RegularExpressions.Regex.Replace(
                    text,
                    @"^(\s*)(private\s+[^;\r\n]+__pbn__[a-zA-Z0-9_]+\s*;)",
                    $"$1[global::LightProto.ProtoIgnore]{Environment.NewLine}$1$2",
                    System.Text.RegularExpressions.RegexOptions.Multiline
                );

                File.WriteAllText( arguments.Output, text );
            }

            return 0;
        }

        static FileDescriptorSet ParseFiles( Options arguments )
        {
            var set = new FileDescriptorSet
            {
                DefaultPackage = arguments.Namespace
            };
            set.AddImportPath( Path.GetDirectoryName( arguments.ProtobufPath ) );

            var fileName = Path.GetFileName( arguments.ProtobufPath );
            if ( !set.Add( fileName, includeInOutput: true ) )
            {
                Console.Error.WriteLine( $"Could not find file '{fileName}'." );
                return null;
            }

            set.Process();
            return set;
        }

        static FileDescriptorSet ReparseFiles( Options arguments, FileDescriptorSet firstPass )
        {
            var set = new FileDescriptorSet
            {
                DefaultPackage = firstPass.DefaultPackage
            };

            set.AddImportPath( Path.GetDirectoryName( arguments.ProtobufPath ) );

            foreach ( var file in firstPass.Files )
            {
                if ( string.IsNullOrEmpty( file.Syntax ) )
                {
                    file.Syntax = "proto2";
                }
                set.Files.Add( file );
            }

            set.Process();
            return set;
        }

        static void PrintErrors( ProtoBuf.Reflection.Error[] errors, out int numErrors, out bool reparse )
        {
            numErrors = 0;
            reparse = false;

            if ( errors.Length > 0 )
            {
                foreach ( var error in errors )
                {
                    if ( error.IsError )
                    {
                        numErrors++;
                    }

                    if ( error.IsWarning && error.Message.StartsWith( "no syntax specified;", StringComparison.Ordinal ) )
                    {
                        reparse = true;
                    }
                    else if ( !error.IsWarning || !error.Message.StartsWith( "import not used:", StringComparison.Ordinal ) )
                    {
                        Console.Error.WriteLine( $"{error.File} ({error.LineNumber}, {error.ColumnNumber}): {error.Message}" );
                    }
                }
            }
        }
    }

    class Options
    {
        [Option( "namespace" )]
        public string Namespace { get; set; }

        [Option( "proto", Required = true )]
        public string ProtobufPath { get; set; }

        [Option( "output", Required = true )]
        public string Output { get; set; }
    }
}
