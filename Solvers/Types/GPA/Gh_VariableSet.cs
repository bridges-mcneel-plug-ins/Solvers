using System;
using System.Collections.Generic;

using GP = BRIDGES.Solvers.GuidedProjection;

using GH_Types = Grasshopper.Kernel.Types;


namespace Solvers.Types.GPA
{
    /// <summary>
    /// Class defining a grasshopper type representing a <see cref="GP.VariableSet"/>.
    /// </summary>
    public class Gh_Set : GH_Types.IGH_Goo
    {
        #region Fields

        /// <summary>
        /// List of components of the set variables.
        /// </summary>
        private List<double> _components;

        #endregion

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
        public string TypeName => nameof(Gh_Set);

        /// <inheritdoc cref="GH_Types.IGH_Goo.TypeDescription"/>
        public string TypeDescription => string.Format($"Grasshopper type representing a set of variables for the Guided Projection Algorithm.");


        /// <summary>
        /// Custom name of the setof variables.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Gets the unique ID of the set.
        /// </summary>
        public Guid GUID { get; private set; }

        /// <summary>
        /// Gets the common dimension of variables in the set.
        /// </summary>
        public int VariableDimension { get; private set; }

        /// <summary>
        /// Gets the number of variables in the set. 
        /// </summary>
        public int VariableCount => _components.Count / VariableDimension;

        #endregion

        #region Constructors

        /// <summary>
        /// Initialises a new instance of <see cref= "Gh_Set" /> class from another <see cref="Gh_Set"/>.
        /// </summary>
        /// <param name="gh_VariableSet"> <see cref="Gh_Set"/> to duplicate. </param>
        public Gh_Set(Gh_Set gh_VariableSet)
        {
            Name = gh_VariableSet.Name;
            GUID = gh_VariableSet.GUID;
            VariableDimension = gh_VariableSet.VariableDimension;

            _components = new List<double>(gh_VariableSet._components);
        }

        /// <summary>
        /// Initialises a new instance of <see cref= "Gh_Set" /> class from its id, components, variable dimension.
        /// </summary>
        /// <param name="components"> Components of the set variables. </param>
        /// <param name="dimension"> Common dimension of the variables in the set. </param>
        /// <param name="name"> Name of the set of variable. </param>
        public Gh_Set(IList<double> components, int dimension, string name)
        {
            if(components is null) { throw new ArgumentNullException(nameof(components)); }
            if(components.Count == 0 & dimension < 1 & (components.Count % dimension) != 0)
            {
                throw new ArgumentException("The number of components must be a non-zero multiple of the variable dimension.");
            }

            Name = name;
            GUID = Guid.NewGuid();
            VariableDimension = dimension;

            _components = new List<double>(components);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Returns the component at the given index in the set.
        /// </summary>
        /// <param name="componentIndex"> Index of the component to get. </param>
        /// <returns> The component at the given index in the set. </returns>
        internal double GetComponent(int componentIndex) => _components[componentIndex];

        /// <summary>
        /// Returns the components of the variable at the given index.
        /// </summary>
        /// <param name="variableIndex"> Index of the variable to get. </param>
        /// <returns> The components of the variable at the index. </returns>
        public double[] GetVariable(int variableIndex)
        {
            double[] variable = new double[VariableDimension];
            int index = variableIndex * VariableDimension;

            for (int i = 0; i < VariableDimension; i++)
            {
                variable[i] = _components[index + i];
            }

            return variable;
        }


        /// <inheritdoc cref="GH_Types.IGH_Goo.Duplicate()"/>
        public GH_Types.IGH_Goo Duplicate() => new Gh_Set(this);


        /// <inheritdoc cref="GH_Types.IGH_Goo.ScriptVariable()"/>
        public object ScriptVariable() => _components;


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
        public override string ToString()
        {
            string text = Name is null ? "Set" : Name;
            return text + $" (N:{VariableCount}, {VariableDimension}D)";
        }
            

        #endregion
    }
}
