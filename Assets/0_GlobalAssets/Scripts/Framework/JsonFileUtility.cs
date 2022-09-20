using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using UnityEngine;

namespace Tpix.Utilities
{
    public static class JsonFileUtility
    {
        // ------------------------------------------------------------------------
        // Name	:	-
        // Desc	:	-
        // ------------------------------------------------------------------------

        public static string ToJson< T >( T data ) => JsonConvert.SerializeObject( data );

        // ------------------------------------------------------------------------
        // Name	:	-
        // Desc	:	-
        // ------------------------------------------------------------------------

        public static T FromJson< T >( string jsonStr )
        {
            return JsonConvert.DeserializeObject< T >( jsonStr, new JsonSerializerSettings( )
            {
                ContractResolver = new PrivateResolver( ),
                ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor
            } );
        }

        // ------------------------------------------------------------------------
        // Name	:	-
        // Desc	:	-
        // ------------------------------------------------------------------------

        public static bool FileExists( string fileName )
        {
            var filePath = FilePath( fileName );
            return File.Exists( filePath );
        }

        // ------------------------------------------------------------------------
        // Name	:	-
        // Desc	:	-
        // ------------------------------------------------------------------------

        public static T Load< T >( string fileName )
        {
            var filePath = FilePath( fileName );

            if( File.Exists( filePath ) )
                return FromJson< T >( File.ReadAllText( filePath ) );
            else
                return default( T );
        }

        // ------------------------------------------------------------------------
        // Name	:	-
        // Desc	:	-
        // ------------------------------------------------------------------------

        public static void Save< T >( T data, string fileName )
        {
            File.WriteAllText( FilePath( fileName ), ToJson( data ) );
        }

        // ------------------------------------------------------------------------
        // Name	:	-
        // Desc	:	-
        // ------------------------------------------------------------------------

        public static void DeleteFile( string fileName )
        {
            var filePath = FilePath( fileName );

            if( File.Exists( filePath ) )
            {
                File.Delete( filePath );
            }
        }

        // ------------------------------------------------------------------------
        // Name	:	-
        // Desc	:	-
        // ------------------------------------------------------------------------

        static string FilePath( string fileName ) => Path.Combine( Application.persistentDataPath, fileName );
    }

    public class PrivateResolver : DefaultContractResolver 
    {
        protected override JsonProperty CreateProperty( MemberInfo member, MemberSerialization memberSerialization )
        {
            var prop = base.CreateProperty( member, memberSerialization );
            if ( ! prop.Writable ) 
            {
                var property = member as PropertyInfo;
                var hasPrivateSetter = property?.GetSetMethod( true ) != null;
                prop.Writable = hasPrivateSetter;
            }
            return prop;
        }
    }
}