using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Mechanical;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using Prism.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevitAPICreateDucts
{
    public class MVVM
    {
        public ExternalCommandData _commandData;
        public List<DuctType> ductTypes { get; } = new List<DuctType>();

        public DelegateCommand saveCommand { get; }
        public List<Level> Levels { get; } = new List<Level>();
        public double PointCenter { get; set; }
        public List<XYZ> Points { get; } = new List<XYZ>();
        public DuctType SelectedDuctType { get; set; }
        public Level SelectedLevels { get; set; }

        public MVVM(ExternalCommandData commandData)
        {
            _commandData = commandData;
            ductTypes = DuctUtils.GetDuctypes(commandData);
            saveCommand = new DelegateCommand(SaveCommand);
            Levels = LevelsUtils.GetLevels(commandData);
            PointCenter = 100;
            Points = SelectionUtils.GetPoints(_commandData, "Check poind", ObjectSnapTypes.Endpoints);
        }

        private void SaveCommand()
        {
            UIApplication uiapp = _commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Document doc = uidoc.Document;

            if (Points.Count < 2 || SelectedDuctType == null || SelectedLevels == null)
                return;

            var curves = new List<Curve>();
            for (int i = 0; i < Points.Count; i++)
            {
                if (i == 0)
                    continue;

                var prevPoint = Points[i - 1];
                var currentPoint = Points[i];

                Connector connector = new Connector();
                Curve curve = Line.CreateBound(prevPoint, currentPoint);
                curves.Add(curve);
            }

            using (var ts = new Transaction(doc, "Create duct"))
            {
                ts.Start();

                foreach (var curve in curves)
                {
                    Duct.Create(doc,curve,SelectedDuctType.Id, SelectedLevels.Id, );
                }

                ts.Commit();
            }
            return;
        }
        public event EventHandler CloseRequest;
        private void RaiseCloseRequest()
        {
            CloseRequest?.Invoke(this, EventArgs.Empty);
        }
    }
}
