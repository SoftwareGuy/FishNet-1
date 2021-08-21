﻿using Mono.Cecil;
using FishNet.Transporting;

namespace FishNet.CodeGenerating.Helping
{
    internal class TransportHelper
    {
        #region Reflection references.        
        internal TypeReference Channel_TypeRef;
        #endregion

        /// <summary>
        /// Resets cached values.
        /// </summary>
        private void ResetValues()
        {
            Channel_TypeRef = null;
        }


        /// <summary>
        /// Imports references needed by this helper.
        /// </summary>
        /// <param name="moduleDef"></param>
        /// <returns></returns>
        internal bool ImportReferences()
        {
            ResetValues();

            Channel_TypeRef = CodegenSession.Module.ImportReference(typeof(Channel));

            return true;
        }

    }
}