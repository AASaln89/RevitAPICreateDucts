﻿using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevitAPICreateDucts
{
    class SelectionUtils
    {
        public static List<Element> PickObjects(ExternalCommandData commandData, string msg = "Select Elements")
        {
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Document doc = uidoc.Document;

            var selectedObjects = uidoc.Selection.PickObjects(ObjectType.Element, msg);
            List<Element> elementList = selectedObjects.Select(selectedObject => doc.GetElement(selectedObject)).ToList();
            return elementList;
        }

        public static List<XYZ> GetPoints(ExternalCommandData commandData, string message, ObjectSnapTypes objectSnapTypes)
        {
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;

            List<XYZ> points = new List<XYZ>();
            while (true)
            {
                XYZ pickedPoints = null;
                try
                {
                    pickedPoints = uidoc.Selection.PickPoint(objectSnapTypes, message);
                }
                catch (OperationCanceledException ex)
                {
                    break;
                }
                points.Add(pickedPoints);
            }
            return points;
        }
    }
}