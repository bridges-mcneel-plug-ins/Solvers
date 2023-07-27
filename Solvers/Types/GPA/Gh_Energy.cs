using System;
using System.Collections.Generic;

using GP = BRIDGES.Solvers.GuidedProjection;

using GH_Types = Grasshopper.Kernel.Types;


namespace Solvers.Types.GPA
{
    /// <summary>
    /// Class defining a grasshopper type, which represents an energy.
    /// </summary>
    public class Gh_Energy : GH_Types.IGH_Goo
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
        public string TypeName => nameof(Gh_Energy);

        /// <inheritdoc cref="GH_Types.IGH_Goo.TypeDescription"/>
        public string TypeDescription => string.Format($"Grasshopper type representing an energy for the Guided Projection Algorithm.");


        /// <summary>
        /// Type of the energy.
        /// </summary>
        public GP.Interfaces.IEnergyType Type { get; private set; }

        /// <summary>
        /// Variables for the energy.
        /// </summary>
        public List<Gh_Variable> Variables { get; private set; }

        /// <summary>
        /// Weight of the energy.
        /// </summary>
        public double Weight { get; private set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initialises a new instance of <see cref= "Gh_Energy" /> class from another <see cref="Gh_Energy"/>.
        /// </summary>
        /// <param name="gh_Energy"> <see cref="Gh_Energy"/> to duplicate. </param>
        public Gh_Energy(Gh_Energy gh_Energy)
        {
            this.Type = gh_Energy.Type;
            this.Variables = new List<Gh_Variable>(gh_Energy.Variables);

            this.Weight = gh_Energy.Weight;
        }

        /// <summary>
        /// Initialises a new instance of <see cref= "Gh_Energy" /> class from its name, id, components and variable dimension.
        /// </summary>
        /// <param name="type"> Type of the energy. </param>
        /// <param name="variables"> Variables for the energy. </param>
        /// <param name="weight"> Weight of the energy. </param>
        public Gh_Energy(GP.Interfaces.IEnergyType type, List<Gh_Variable> variables, double weight)
        {
            this.Type = type;
            this.Variables = new List<Gh_Variable>(variables);

            this.Weight = weight;
        }

        #endregion

        #region Public Methods

        /// <inheritdoc cref="GH_Types.IGH_Goo.Duplicate()"/>
        public GH_Types.IGH_Goo Duplicate() => new Gh_Energy(this);


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
