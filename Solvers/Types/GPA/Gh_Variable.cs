using System;

using GH_Types = Grasshopper.Kernel.Types;


namespace Solvers.Types.GPA
{
    /// <summary>
    /// Class defining a grasshopper type, which represents a variable.
    /// </summary>
    /// <remarks> The variable is represented as a pair (S, I) where:
    /// <list type="bullet">
    /// <item><term>S</term> Set of variables.</item>
    /// <item><term>I</term> Index of the variable in the set.</item>
    /// </list>
    /// </remarks>
    public class Gh_Variable : GH_Types.IGH_Goo
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
        public string TypeName => nameof(Gh_Variable);

        /// <inheritdoc cref="GH_Types.IGH_Goo.TypeDescription"/>
        public string TypeDescription => string.Format($"Grasshopper type representing a variable for the Guided Projection Algorithm.");


        /// <summary>
        /// Name of the set of variables.
        /// </summary>
        public string SetName { get; private set; }

        /// <summary>
        /// Identifier of the set of variables.
        /// </summary>
        public Guid SetGuid { get; private set; }

        /// <summary>
        /// Index of the variable in the set.
        /// </summary>
        public int Index { get; private set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initialises a new instance of <see cref= "Gh_Variable" /> class from another <see cref="Gh_Variable"/>.
        /// </summary>
        /// <param name="gh_SetIndexPair"> <see cref="Gh_Variable"/> to duplicate. </param>
        public Gh_Variable(Gh_Variable gh_SetIndexPair)
        {
            SetName = gh_SetIndexPair.SetName;
            SetGuid = gh_SetIndexPair.SetGuid;
            Index = gh_SetIndexPair.Index;
        }

        /// <summary>
        /// Initialises a new instance of <see cref= "Gh_Set" /> class from its name, id, components and variable dimension.
        /// </summary>
        /// <param name="name"> Name of the set of variables. </param>
        /// <param name="guid"> Unique identifier of the set of variables. </param>
        /// <param name="count"> Number of variable in the set.</param>
        /// <param name="index"> Index of the variable in the set. </param>
        /// <exception cref="ArgumentOutOfRangeException"> The index is outside the bound of the set. </exception>
        public Gh_Variable(string name, Guid guid, int count, int index)
        {
            if (index < 0 || count < index) { throw new ArgumentOutOfRangeException("The index is outside the bound of the set."); }

            SetName = name;
            SetGuid = guid;
            Index = index;
        }

        #endregion

        #region Public Methods

        /// <inheritdoc cref="GH_Types.IGH_Goo.Duplicate()"/>
        public GH_Types.IGH_Goo Duplicate() => new Gh_Variable(this);


        /// <inheritdoc cref="GH_Types.IGH_Goo.ScriptVariable()"/>
        public object ScriptVariable() => (SetGuid, Index);


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
        public override string ToString() => SetName is null ? $"({SetGuid}, {Index})" : $"({SetName}, {Index})";

        #endregion
    }
}
