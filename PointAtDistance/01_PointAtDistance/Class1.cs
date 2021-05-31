using System;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.EditorInput;
using AcadApp = Autodesk.AutoCAD.ApplicationServices.Application;

namespace _01_PointAtDistance
{
    public class Class1
    {
        [CommandMethod("tt")]
        public void PointAtDistance()
        {
            Document acDoc = AcadApp.DocumentManager.MdiActiveDocument;
            Database acCurDb = acDoc.Database;

            PromptEntityOptions pEtOpts = new PromptEntityOptions("\n选择路线中心线");
            PromptEntityResult pEtRes = acDoc.Editor.GetEntity(pEtOpts);
            if (pEtRes.Status != PromptStatus.OK) return;
            ObjectId entCur = pEtRes.ObjectId;
            PromptDoubleOptions pDblOpts = new PromptDoubleOptions("\n请输入桩号点");
            PromptDoubleResult pDblRes = acDoc.Editor.GetDouble(pDblOpts);
            Double StationMark = pDblRes.Value;

            using(Transaction acTrans = acCurDb.TransactionManager.StartTransaction())
            {
                BlockTable acBlkTbl = acTrans.GetObject(acCurDb.BlockTableId, OpenMode.ForRead) as BlockTable;
                BlockTableRecord acBlkTblRec = acTrans.GetObject(acBlkTbl[BlockTableRecord.ModelSpace],OpenMode.ForWrite) as BlockTableRecord;

                Curve RoadCenterLine = acTrans.GetObject(entCur, OpenMode.ForRead) as Curve;
                Point3d StationMarkPoint = RoadCenterLine.GetPointAtDist(StationMark);
                Point3d RefPoint = new Point3d(StationMarkPoint.X - 100, StationMarkPoint.Y - 100, StationMarkPoint.Z);
                Line acline = new Line(StationMarkPoint, RefPoint);
                acBlkTblRec.AppendEntity(acline);
                acTrans.AddNewlyCreatedDBObject(acline, true);
                acTrans.Commit();
            }
        }
    }
}
