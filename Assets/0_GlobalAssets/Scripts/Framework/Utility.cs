using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Tpix.Utilities
{
    public static class Utility
    {
        // ------------------------------------------------------------------------
        // Name :   -
        // Desc :   -
        // ------------------------------------------------------------------------

	    public static List< T > FindInterfacesOfType< T >( )
        {
		    return GameObject.FindObjectsOfType< MonoBehaviour >( ).OfType< T >( ).ToList( );
        }
    }
}