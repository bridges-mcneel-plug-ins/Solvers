using System;
using System.Collections.Generic;
using System.Windows.Forms;

using GP = BRIDGES.Solvers.GuidedProjection;
using Euc3D = BRIDGES.Geometry.Euclidean3D;

using GH_Kernel = Grasshopper.Kernel;
using GH_Data = Grasshopper.Kernel.Data;
using GH_Types = Grasshopper.Kernel.Types;
using GH_Param = Grasshopper.Kernel.Parameters;

using Gh_Types_Euc3D = BRIDGES.McNeel.Grasshopper.Types.Geometry.Euclidean3D;
using Gh_Disp_Euc3D = BRIDGES.McNeel.Grasshopper.Display.Geometry.Euclidean3D;

using S_Types = Solvers.Types.GPA;
using S_Param = Solvers.Parameters.GPA;


namespace Solvers.Components.GPA.Variable
{
    /// <summary>
    /// A grasshopper component creating a <see cref="S_Types.Gh_VariableSet"/>.
    /// </summary>
    internal class Comp_ConstructVariableSet : GH_Kernel.GH_Component
    {
        /// <summary>
        /// Enum type giving different configuration for the component
        /// </summary>
        private enum ComponentConfiguration : byte
        {
            Generic = 0,
            BRIDGES = 10,
        }

        #region Fields
        
        /// <summary>
        /// State of the component.
        /// </summary>
        private ComponentConfiguration _configuration;

        /// <summary>
        /// Evaluate whether the input format is valid for the current configuration.
        /// </summary>
        private bool _isInputFormatValid;

        #endregion

        #region Constructors

        /// <summary>
        /// Initialises a new instance of the <see cref="Comp_ConstructVariableSet"/> class.
        /// </summary>
        public Comp_ConstructVariableSet()
          : base("Construct a Set", "Set",
              "Construct a set of variables for the Guided Projection Algorithm.",
              Settings.CategoryName, Settings.SubCategoryName[Solvers.SubCategory.GPA])
        {
            _configuration = ComponentConfiguration.Generic;

            this.Params.ParameterSourcesChanged += OnParameterSourceChanged;
        }

        #endregion

        #region Methods

        // ---------- For Message Handling ---------- //

        private void OnParameterSourceChanged(object sender, GH_Kernel.GH_ParamServerEventArgs e)
        {
            int index = e.ParameterIndex;

            // Ensures that the message is removed when there is no input value.
            if (index == 1 & this.Params.Input[index].SourceCount == 0) { Message = null; }
        }


        // ---------- For Component Mutation ---------- //

        /// <summary>
        /// Method called when the selected item in the Component Menu is changed.
        /// </summary>
        /// <param name="sender"> New selected <see cref="ToolStripMenuItem"/> of the component menu.</param>
        /// <param name="e"><see langword="null"/> value.</param>
        /// <exception cref="NotImplementedException">The name of the variable was not recognised.</exception>
        private void MenuItemClicked(object sender, EventArgs e)
        {
            ToolStripMenuItem item = (ToolStripMenuItem)sender;
            string itemName = item.AccessibilityObject.Name;

            RecordUndoEvent(itemName);

            if(itemName == ComponentConfiguration.Generic.ToString()) { _configuration = ComponentConfiguration.Generic; }
            else if (itemName == ComponentConfiguration.BRIDGES.ToString()) { _configuration = ComponentConfiguration.BRIDGES; }
            else{ throw new NotImplementedException("The name of the variable was not recognised."); }

            ChangeInputType();

            ExpireSolution(true);
        }

        /// <summary>
        /// Changes the component input depending of the current <see cref="_configuration"/>.
        /// </summary>
        /// <exception cref="NotImplementedException"> The given component mode is not implemented. </exception>
        private void ChangeInputType()
        {
            // Store the information about the existing input
            bool isReversed = this.Params.Input[1].Reverse;
            bool isSimplified = this.Params.Input[1].Simplify;
            GH_Kernel.GH_DataMapping datamapping = this.Params.Input[1].DataMapping;
            IList<GH_Kernel.IGH_Param> sources = new List<GH_Kernel.IGH_Param>(this.Params.Input[1].Sources);

            // Clean the input of the component
            this.Params.Input[1].RemoveAllSources();
            this.Params.Input.RemoveAt(1);

            // Create the new input according to variable type
            switch (_configuration)
            {
                case ComponentConfiguration.Generic:
                    GH_Param.Param_Number param_Generic = new GH_Param.Param_Number();
                    param_Generic.Name = "Values";
                    param_Generic.NickName = "V";
                    param_Generic.Description = "Values for the set variables, represented as lists of numerical values.";
                    param_Generic.Access = GH_Kernel.GH_ParamAccess.tree;
                    Params.RegisterInputParam(param_Generic);
                    break;
                case ComponentConfiguration.BRIDGES :
                    GH_Param.Param_GenericObject param_GenericObject = new GH_Param.Param_GenericObject();
                    param_GenericObject.Name = "Values";
                    param_GenericObject.NickName = "V";
                    param_GenericObject.Description = "Values for the set's variables, that are (convertible to) types for BRIDGES.";
                    param_GenericObject.Access = GH_Kernel.GH_ParamAccess.list;
                    Params.RegisterInputParam(param_GenericObject);
                    break;
                default:
                    throw new NotImplementedException("The input change for the given component mode is not implemented.");
            }

            // Add the sources to the new input & set the other information
            for (int i = 0; i < sources.Count; i++) { this.Params.Input[1].AddSource(sources[i]); }

            this.Params.Input[1].Reverse = isReversed;
            this.Params.Input[1].Simplify = isSimplified;
            this.Params.Input[1].DataMapping = datamapping;

            this.OnAttributesChanged();
            this.OnSolutionExpired(true);


        }


        // ---------- For SolveInstance ---------- //

        /// <summary>
        /// This function will be called (successively) from within the SolveInstance method of this component if the component <see cref="_configuration"/> is <see cref="ComponentConfiguration.Generic"/>.
        /// </summary>
        /// <param name="DA"> Data Access object. Use this object to retrieve data from input parameters and assign data to output parameters. </param>
        /// <exception cref="InvalidCastException"> "The input could not be casted to GH_Number." </exception>
        /// <exception cref="ArgumentNullException"> The input containing the values is null. </exception>
        /// <exception cref="ArgumentException"> The generic variable do not have the same dimension. All the variables of a set must have the same dimension. </exception>
        private List<GP.Variable> CreateSet_Generic(GH_Kernel.IGH_DataAccess DA)
        {
            // ----- Get Inputs ----- //

            if (!DA.GetDataTree(1, out GH_Data.GH_Structure<GH_Types.GH_Number> tree_Numbers))  {  return null; };
            // GetDataTree does not throw an exception if the conversion failed, but changes a Runtime Message Level.
            // To avoid any NullReferenceException, the level of the Runtime Message is checked.
            if (this.RuntimeMessageLevel == GH_Kernel.GH_RuntimeMessageLevel.Error)
            {
                throw new InvalidCastException("The input could not be casted to GH_Number.");
            }

            // ----- Core ----- //

            List<GP.Variable> variables;

            // Verifications
            if (tree_Numbers.IsEmpty) { throw new ArgumentNullException(nameof(tree_Numbers), "The input containing the values is null."); }

            // If the input Gh_Structure is actually a list, then each variable corresponds to an item in the list.
            if (tree_Numbers.Branches.Count == 1)
            {
                variables = new List<GP.Variable>(tree_Numbers.DataCount);

                List<GH_Types.GH_Number> branch = tree_Numbers[0];
                for (int i = 0; i < branch.Count; i++)
                {
                    GP.Variable variable = new GP.Variable(branch[i].Value);
                    variables.Add(variable);
                }
            }

            // The input Gh_Structure has several branch.
            // Each variable corresponds to a branch, and the variable components corresponds to the items in the branch.
            else
            {
                int variableCount = tree_Numbers.Branches.Count;
                int variableDimension = tree_Numbers[0].Count;
                variables = new List<GP.Variable>(variableCount);

                for (int i_Var = 0; i_Var < variableCount; i_Var++)
                {
                    List<GH_Types.GH_Number> branch = tree_Numbers[i_Var];

                    if (branch.Count != variableDimension)
                    {
                        throw new ArgumentException("The generic variable do not have the same dimension. All the variables of a set must have the same dimension.");
                    }

                    double[] components = new double[variableDimension];
                    for (int i_Comp = 0; i_Comp < variableDimension; i_Comp++) { components[i_Comp] = branch[i_Comp].Value; }

                    GP.Variable variable = new GP.Variable(components);
                    variables.Add(variable);
                }
            }

            /******************** Set Output ********************/

            Message = "Generic";
            return variables;
        }

        /// <summary>
        /// This function will be called (successively) from within the SolveInstance method of this component if the component <see cref="_configuration"/> is <see cref="ComponentConfiguration.BRIDGES"/>.
        /// </summary>
        /// <param name="DA"> Data Access object. Use this object to retrieve data from input parameters and assign data to output parameters. </param>
        /// <exception cref="ArgumentException"> The generic variable do not have the same dimension. All the variables of a set must have the same dimension. </exception>
        private List<GP.Variable> CreateSet_BRIDGES(GH_Kernel.IGH_DataAccess DA)
        {
            /******************** Get Inputs ********************/

            List<GH_Types.IGH_Goo> inputs = new List<GH_Types.IGH_Goo>();

            /******************** Get Inputs ********************/

            if (!DA.GetDataList(1, inputs) || inputs.Count == 0) { return null; };

            /******************** Core ********************/

            List<GP.Variable> variables;

            GH_Types.IGH_Goo input = inputs[0];

            GH_Types.GH_Number gh_Number = new GH_Types.GH_Number(0);
            Gh_Types_Euc3D.Gh_Point gh_Point = new Gh_Types_Euc3D.Gh_Point();
            Gh_Types_Euc3D.Gh_Vector gh_Vector = new Gh_Types_Euc3D.Gh_Vector();

            // System types
            if (gh_Number.CastFrom(input))
            {
                variables = CreateSet_FromGHDouble(inputs);
                Message = "Double";
            }
            // BRIDGES types
            else if (gh_Point.CastFrom(input))
            {
                variables = CreateSet_FromGhPoint(inputs);
                Message = "Point";
            }
            else if (gh_Vector.CastFrom(input))
            {
                variables = CreateSet_FromGhVector(inputs);
                Message = "Vector";
            }
            else { variables = null; }

            return variables;
        }


        #region For CreateSet_BRIDGES

        /// <summary>
        /// Create the set of variables from a list of <see cref="GH_Types.GH_Number"/>
        /// </summary>
        /// <param name="inputs"> Variables from which to get the components. </param>
        /// <returns></returns>
        private List<GP.Variable> CreateSet_FromGHDouble(List<GH_Types.IGH_Goo> inputs)
        {
            List<GP.Variable> variables = new List<GP.Variable>(inputs.Count);

            GH_Types.GH_Number gh_Number = new GH_Types.GH_Number(0);
            for (int i = 0; i < inputs.Count; i++)
            {
                gh_Number.CastFrom(inputs[i]);

                GP.Variable variable = new GP.Variable(gh_Number.Value);

                variables.Add(variable);
            }

            return variables;
        }


        /// <summary>
        /// Create the set of variables from a list of <see cref="Gh_Types_Euc3D.Gh_Point"/>
        /// </summary>
        /// <param name="inputs"> Variables from which to get the components. </param>
        /// <returns></returns>
        private List<GP.Variable> CreateSet_FromGhPoint(List<GH_Types.IGH_Goo> inputs)
        {
            List<GP.Variable> variables = new List<GP.Variable>(inputs.Count);

            Gh_Types_Euc3D.Gh_Point gh_Point = new Gh_Types_Euc3D.Gh_Point();
            for (int i = 0; i < inputs.Count; i++)
            {
                gh_Point.CastFrom(inputs[i]);

                GP.Variable variable = new GP.Variable(gh_Point.Value.X, gh_Point.Value.Y, gh_Point.Value.Z);

                variables.Add(variable);
            }

            return variables;
        }

        /// <summary>
        /// Create the set of variables from a list of <see cref="Gh_Types_Euc3D.Gh_Vector"/>
        /// </summary>
        /// <param name="inputs"> Variables from which to get the components. </param>
        /// <returns></returns>
        private List<GP.Variable> CreateSet_FromGhVector(List<GH_Types.IGH_Goo> inputs)
        {
            List<GP.Variable> variables = new List<GP.Variable>(inputs.Count);

            Gh_Types_Euc3D.Gh_Vector gh_Vector = new Gh_Types_Euc3D.Gh_Vector();
            for (int i = 0; i < inputs.Count; i++)
            {
                gh_Vector.CastFrom(inputs[i]);

                GP.Variable variable = new GP.Variable(gh_Vector.Value.X, gh_Vector.Value.Y, gh_Vector.Value.Z);

                variables.Add(variable);
            }

            return variables;
        }

        #endregion

        #endregion


        #region Override : GH_Component

        /// <inheritdoc cref="GH_Kernel.GH_Component.IsPreviewCapable"/>
        public override bool IsPreviewCapable => false;


        /// <inheritdoc cref="GH_Kernel.GH_Component.AppendAdditionalComponentMenuItems(ToolStripDropDown)"/>
        protected override void AppendAdditionalComponentMenuItems(ToolStripDropDown menu)
        {
            // Append the item to the menu, making sure it's always enabled and checked if Absolute is True.
            ToolStripMenuItem item_Generic = Menu_AppendItem(menu, ComponentConfiguration.Generic.ToString(), MenuItemClicked, true, _configuration == ComponentConfiguration.Generic);
            ToolStripMenuItem item_BRIDGES = Menu_AppendItem(menu, ComponentConfiguration.BRIDGES.ToString(), MenuItemClicked, true, _configuration == ComponentConfiguration.BRIDGES);

            // Specifically assign a tooltip text to the menu item.
            item_Generic.ToolTipText = "The set variables are generic values, which are represented by lists of numerical values.";
            item_BRIDGES.ToolTipText = "The set variables are (convertible to) types for BRIDGES.";

            // Activate the checking of the mode.
            item_Generic.CheckOnClick = true;
            item_BRIDGES.CheckOnClick = true;
        }


        /// <inheritdoc cref="GH_Kernel.GH_Component.AddedToDocument(GH_Kernel.GH_Document)"/>
        /// <remarks>
        /// Allows to ensure that the inputs are updated according to <see cref="_configuration"/> when the component is copied and pasted.
        /// </remarks>
        public override void AddedToDocument(GH_Kernel.GH_Document document)
        {
            ChangeInputType();
        }

        /// <inheritdoc cref="GH_Kernel.GH_Component.RegisterInputParams(GH_InputParamManager)"/>
        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddTextParameter("Name", "N", "Name of the set of variables", GH_Kernel.GH_ParamAccess.item);
            pManager.AddNumberParameter("Generic Values", "G", "Values for the set variables, represented as lists of numerical values.", GH_Kernel.GH_ParamAccess.tree);
        }

        /// <inheritdoc cref="GH_Kernel.GH_Component.RegisterOutputParams(GH_OutputParamManager)"/>
        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddParameter(new S_Param.Param_VariableSet(), "Set of variables", "S", "Set of variables initialised with the given values.", GH_Kernel.GH_ParamAccess.item);
            pManager.AddParameter(new S_Param.Param_Variable(), "Variables", "V", "Variables composing the newly constructed Set.", GH_Kernel.GH_ParamAccess.list);
        }


        /// <inheritdoc cref="GH_Kernel.GH_Component.BeforeSolveInstance()"/>
        protected override void BeforeSolveInstance()
        {
            Message = null;

            // If the component's mode is set to BRIDGES, then the input must be a list.
            if (_configuration == ComponentConfiguration.BRIDGES & 1 < this.Params.Input[1].VolatileData.PathCount)
            {
                this.AddRuntimeMessage(GH_Kernel.GH_RuntimeMessageLevel.Error, "The inputs must be a list.");
                _isInputFormatValid = false;
            }
            else
            {
                _isInputFormatValid = true;
                base.BeforeSolveInstance();
            }
            
        }

        /// <inheritdoc cref="GH_Kernel.GH_Component.SolveInstance(GH_Kernel.IGH_DataAccess)"/>
        protected override void SolveInstance(GH_Kernel.IGH_DataAccess DA)
        {
            /******************** Get Inputs ********************/

            string name = null;
            DA.GetData(0, ref name);

            if (!_isInputFormatValid) { return; }

            /******************** Core ********************/

            List<GP.Variable> set;

            if (_configuration == ComponentConfiguration.Generic) { set = CreateSet_Generic(DA); }
            else if (_configuration == ComponentConfiguration.BRIDGES) { set = CreateSet_BRIDGES(DA); }
            else { throw new NotImplementedException("The solve instance method for the given component mode is not implemented."); }

            if(set is null) { return; }

            /******************** Set Output ********************/

            S_Types.Gh_VariableSet gh_Set = new S_Types.Gh_VariableSet(set, name);

            List<S_Types.Gh_Variable> gh_Variables = new List<S_Types.Gh_Variable>(set.Count);

            S_Types.Gh_Variable variable;
            for (int i = 0; i < set.Count; i++)
            {
                variable = new S_Types.Gh_Variable(set[i]);
                gh_Variables.Add(variable);
            }

            DA.SetData(0, gh_Set);
            DA.SetDataList(1, gh_Variables);
        }


        /// <summary>
        /// When a component is saved or copied, this method allows to add costum data to the file.
        /// </summary>
        /// <param name="writer"></param>
        /// <returns>True if the component was properly saved, otherwise false.</returns>
        public override bool Write(GH_IO.Serialization.GH_IWriter writer)
        {
            // First add our own field.
            writer.SetInt32("Mode", (int)_configuration);

            // Then call the base class implementation.
            return base.Write(writer);
        }

        /// <summary>
        /// When a component is opened or pasted, this method reads the costum data saved to the file.
        /// </summary>
        /// <param name="reader"></param>
        /// <returns>True if the component was properly read, otherwise false.</returns>
        public override bool Read(GH_IO.Serialization.GH_IReader reader)
        {
            // First read our own field.
            _configuration = (ComponentConfiguration)reader.GetInt32("Mode");

            // Then call the base class implementation.
            return base.Read(reader);
        }

        #endregion

        #region Override : GH_DocumentObject

        /// <inheritdoc cref="GH_Kernel.GH_DocumentObject.ComponentGuid"/>
        public override Guid ComponentGuid
        {
            get { return new Guid("{796723F7-9C99-430D-B663-864179720259}"); }
        }

        /// <inheritdoc cref="GH_Kernel.GH_DocumentObject.Icon"/>
        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                return null;
            }
        }

        /// <inheritdoc cref="GH_Kernel.GH_DocumentObject.Exposure"/>
        public override GH_Kernel.GH_Exposure Exposure
        {
            get { return (GH_Kernel.GH_Exposure)TabExposure.Variable; }
        }

        /// <inheritdoc cref="GH_Kernel.GH_DocumentObject.CreateAttributes()"/>
        public override void CreateAttributes()
        {
            m_attributes = new Gh_Disp_Euc3D.ComponentAttributes(this);
        }

        #endregion
    }
}
