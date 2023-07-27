﻿using System;

using GP = BRIDGES.Solvers.GuidedProjection;

using GH_Types = Grasshopper.Kernel.Types;


namespace Solvers.Types.GPA
{
    /// <summary>
    /// Class defining a grasshopper type for an <see cref="GP.GuidedProjectionAlgorithm"/>.
    /// </summary>
    public class Gh_Model : GH_Types.GH_Goo<GP.GuidedProjectionAlgorithm>
    {
        #region Constructors

        /// <summary>
        /// Initialises a new instance of <see cref= "Gh_Model" /> class.
        /// </summary>
        public Gh_Model() { }

        /// <summary>
        /// Initialises a new instance of <see cref= "Gh_Model" /> class from another <see cref="Gh_Model"/>..
        /// </summary>
        /// <param name="gh_Model"> <see cref="Gh_Model"/> to duplicate. </param>
        public Gh_Model(Gh_Model gh_Model)
        {
            this.Value = gh_Model.Value;
        }

        /// <summary>
        /// Initialises a new instance of <see cref="Gh_Model"/> class from a <see cref="GP.GuidedProjectionAlgorithm"/>.
        /// </summary>
        /// <param name="gpa"> <see cref="GP.GuidedProjectionAlgorithm"/> for the grasshopper type.</param>
        public Gh_Model(GP.GuidedProjectionAlgorithm gpa)
        {
            this.Value = gpa;
        }

        #endregion


        #region Override : GH_Goo<>

        /********** Properties **********/

        /// <inheritdoc cref="GH_Types.GH_Goo{T}.IsValid"/>
        public override bool IsValid  => true;

        /// <inheritdoc cref="GH_Types.GH_Goo{T}.TypeDescription"/>
        public override string TypeDescription => $"Grasshopper type containing a {typeof(GP.GuidedProjectionAlgorithm)}.";

        /// <inheritdoc cref="GH_Types.GH_Goo{T}.TypeName"/>
        public override string TypeName => nameof(Gh_Model);


        /********** Methods **********/

        /// <inheritdoc cref="GH_Types.GH_Goo{T}.ToString"/>
        public override string ToString()
        {
            return $"(V:{Value.X.Size}, E:?, C:?)";
        }

        /// <inheritdoc cref="GH_Types.GH_Goo{T}.Duplicate"/>
        public override GH_Types.IGH_Goo Duplicate() => new Gh_Model(this);


        /// <inheritdoc cref="GH_Types.GH_Goo{T}.CastFrom(object)"/>
        public override bool CastFrom(object source)
        {
            if (source == null) { return false; }

            var type = source.GetType();


            /******************** BRIDGES Objects ********************/

            // Cast a GP.GuidedProjectionAlgorithm to a Gh_Model
            if (typeof(GP.GuidedProjectionAlgorithm).IsAssignableFrom(type))
            {
                this.Value = (GP.GuidedProjectionAlgorithm)source;

                return true;
            }


            /******************** Otherwise ********************/

            return false;
        }

        /// <inheritdoc cref="GH_Types.GH_Goo{T}.CastTo{Q}(ref Q)"/>
        public override bool CastTo<T>(ref T target)
        {
            /******************** BRIDGES Objects ********************/

            // Casts a Gh_Model to a GP.GuidedProjectionAlgorithm
            if (typeof(T).IsAssignableFrom(typeof(GP.GuidedProjectionAlgorithm)))
            {
                target = (T)(object)this.Value;

                return true;
            }

            /******************** Otherwise ********************/

            return false;
        }

        #endregion
    }
}
