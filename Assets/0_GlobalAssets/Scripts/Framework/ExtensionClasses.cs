using System;
using System.Collections.Generic;
using UnityEngine;

namespace Tpix.Utilities
{
    public static class Extensions
    {
        private const double Epsilon = 1e-10;

        // ------------------------------------------------------------------------
        // Name :   -
        // Desc :   -
        // ------------------------------------------------------------------------

        public static bool IsZero( this float d )
        {
            return Mathf.Abs( d ) < Epsilon;
        }

        // ------------------------------------------------------------------------
        // Name :   -
        // Desc :   -
        // ------------------------------------------------------------------------

        public static List< int > AllIndexesOf( this string str, string value ) 
        {
            if( string.IsNullOrEmpty( value ) )
            {
                throw new ArgumentException( "the string to find may not be empty", "value");
            }

            List< int > indexes = new List< int >( );

            for( int index = 0; ; index += value.Length )
            {
                index = str.IndexOf( value, index );
                if( index == -1 ) return indexes;

                indexes.Add( index );
            }
        }

        // ------------------------------------------------------------------------
        // Name :   -
        // Desc :   -
        // ------------------------------------------------------------------------

        public static List< T > Shuffle< T >( this List< T > list )  
        {  
            System.Random rng = new System.Random( );

            int n = list.Count;  
            while ( n > 1 ) 
            {  
                n--;  
                int k = rng.Next( n + 1 );  
                T value = list[ k ];  
                list[ k ] = list[ n ];  
                list[ n ] = value;  
            }

            return list;
        }

        // ------------------------------------------------------------------------
        // Name :   -
        // Desc :   -
        // ------------------------------------------------------------------------

        public static string RemoveQuotes( this string Value )
        {
            return Value.Replace( "\"", "" );
        }

        // ------------------------------------------------------------------------
        // Name :   -
        // Desc :   -
        // ------------------------------------------------------------------------

        public static DateTime StartTimeOfWeek( this DateTime dt, DayOfWeek startOfWeek )
        {
            int diff = ( 7 + ( dt.DayOfWeek - startOfWeek ) ) % 7;
            return dt.AddDays( -1 * diff ).Date;
        }
    }
}
