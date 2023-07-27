using System;
using System.Collections.Generic;

using GP = BRIDGES.Solvers.GuidedProjection;

using GH_Types = Grasshopper.Kernel.Types;


namespace Solvers.Types.GPA
{
    /// <summary>
    /// Class defining a grasshopper type, which represents an energy.
    /// </summary>
    public class Gh_QuadraticConstraint : GH_Types.IGH_Goo
    {
        #region Properties

        /// <inheritdoc cref="GH_Types.IGH_Goo.IsValid"/>
        public bool IsValid => true;

        /// <inheritdoc cref="GH_Types.IGH_Goo.IsValidWhyNot"/>
        public string IsValidWhyNot
        {
            get
            {
                if (IsValid) { return string.Empty; }

                return $"This {TypeName} is not valid, but I don't know why.";
            }
        }

        /// <inheritdoc cref="GH_Types.IGH_Goo.TypeName"/>
        public string TypeName => nameof(Gh_QuadraticConstraint);

        /// <inheritdoc cref="GH_Types.IGH_Goo.TypeDescription"/>
        public string TypeDescription => string.Format($"Grasshopper type representing a quadratic constraint for the Guided Projection Algorithm.");


        /// <summary>
        /// Type of the quadratic constraint.
        /// </summary>
        public GP.Interfaces.IQuadraticConstraintType Type { get; private set; }

        /// <summary>
        /// Variables for the quadratic constraint.
        /// </summary>
        public List<Gh_Variable> Variables { get; private set; }

        /// <summary>
        /// Weight of the quadratic constraint.
        /// </summary>
        public double Weight { get; private set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initialises a new instance of <see cref= "Gh_QuadraticConstraint" /> class from another <see cref="Gh_QuadraticConstraint"/>.
        /// </summary>
        /// <param name="gh_QuadraticConstraint"> <see cref="Gh_QuadraticConstraint"/> to duplicate. </param>
        public Gh_QuadraticConstraint(Gh_QuadraticConstraint gh_QuadraticConstraint)
        {
            this.Type = gh_QuadraticConstraint.Type;
            this.Variables = new List<Gh_Variable>(gh_QuadraticConstraint.Variables);

            this.Weight = gh_QuadraticConstraint.Weight;
        }

        /// <summary>
        /// Initialises a new instance of <see cref= "Gh_QuadraticConstraint" /> class from its name, id, components and variable dimension.
        /// </summary>
        /// <param name="type"> Type of the quadratic constraint. </param>
        /// <param name="variables"> Variables for the quadratic constraint. </param>
        /// <param name="weight"> Weight of the quadratic constraint. </param>
        public Gh_QuadraticConstraint(GP.Interfaces.IQuadraticConstraintType type, List<Gh_Variable> variables, double weight)
        {
            this.Type = type;
            this.Variables = new List<Gh_Variable>(variables);

            this.Weight = weight;
        }

        #endregion

        #region Public Methods

        /// <inheritdoc cref="GH_Types.IGH_Goo.Duplicate()"/>
        public GH_Types.IGH_Goo Duplicate() => new Gh_QuadraticConstraint(this);


        /// <inheritdoc cref="GH_Types.IGH_Goo.ScriptVariable()"/>
        public object ScriptVariable() => false;


        /// <inheritdoc cref="GH_Types.IGH_Goo.CastFrom(object)"/>
        public bool CastFrom(object source)
        {
            return false;
        }

        /// <inheritdoc cref="GH_Types.IGH_Goo.CastTo{T}(out T)"/>
        public bool CastTo<T>(out T target)
        {
            target = default;

            return false;
        }


        /// <inheritdoc cref="GH_Types.IGH_Goo.EmitProxy()"/>
        public GH_Types.IGH_GooProxy EmitProxy() => null;


        /// <inheritdoc cref="GH_IO.GH_ISerializable.Read(GH_IO.Serialization.GH_IReader)"/>
        public bool Read(GH_IO.Serialization.GH_IReader reader) => true;

        /// <inheritdoc cref="GH_IO.GH_ISerializable.Write(GH_IO.Serialization.GH_IWriter)"/>
        public bool Write(GH_IO.Serialization.GH_IWriter writer) => true;

        #endregion


        #region Override : Object

        /// <inheritdoc cref="GH_Types.IGH_Goo.ToString()"/>
        public override string ToString() => Type.ToString();

        #endregion
    }
}
