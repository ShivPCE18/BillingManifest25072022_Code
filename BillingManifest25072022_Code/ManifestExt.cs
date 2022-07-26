using System;
using PX.Objects;
using PX.Data;
using BillingManifest;
using PX.Objects.SO;
using PX.Objects.IN;

namespace BillingManifest2021051402274
{
    // Acuminator disable once PX1016 ExtensionDoesNotDeclareIsActiveMethod extension should be constantly active
    public class MainfestEntry_Extension : PXGraphExtension<MainfestEntry>
    {
        #region Event Handlers

        //protected void BillingMainfest_RowSelecting(PXCache cache, PXRowSelectingEventArgs e)
        //{

        //    var row = (BillingMainfest)e.Row;
        //    var ext = row.GetExtension<BillingMainfestExt>();

        //    if (row != null)
        //        if (ext.UsrStatus == status.Complete)
        //        {
        //            cache.AllowUpdate = false;
        //            Base.Transactions.Cache.AllowUpdate = false;
        //            Base.Transactions.AllowDelete = false;

        //            // Acuminator disable once PX1070 UiPresentationLogicInEventHandlers [Justification]
        //            PXUIFieldAttribute.SetEnabled<BillingMainfestLine.customerID>(Base.Transactions.Cache, Base.Transactions.Current, false);
        //            // Acuminator disable once PX1070 UiPresentationLogicInEventHandlers [Justification]
        //            PXUIFieldAttribute.SetEnabled<BillingMainfestLine.locationID>(Base.Transactions.Cache, Base.Transactions.Current, false);
        //        }

        //}

        //protected void BillingMainfest_RowPersisting(PXCache cache, PXRowPersistingEventArgs e)
        //{

        //    var row = (BillingMainfest)e.Row;
        //    var ext = row.GetExtension<BillingMainfestExt>();

        //    if (row != null)
        //        if (ext.UsrStatus == status.Complete)
        //        {
        //            cache.AllowUpdate = false;
        //            Base.Transactions.Cache.AllowUpdate = false;
        //        }

        //}

        //protected void BillingMainfestLine_RowPersisting(PXCache cache, PXRowPersistingEventArgs e)
        //{

        //    var row = (BillingMainfestLine)e.Row;

        //    if (row != null)
        //    {
        //        var ext = Base.Document.Current.GetExtension<BillingMainfestExt>();
        //        if (ext != null)
        //            if (ext.UsrStatus == status.Complete)
        //            {
        //                cache.AllowUpdate = false;
        //                Base.Document.AllowUpdate = false;
        //            }
        //    }
        //}

        protected void BillingMainfestLine_RowSelected(PXCache cache, PXRowSelectedEventArgs e)
        {

            var row = (BillingMainfestLine)e.Row;

            if (row != null)
            {

                var ext = row.GetExtension<BillingMainfestLineExt>();
                if (ext != null)
                {
                    SOOrderEntry graph = PXGraph.CreateInstance<SOOrderEntry>();
                    graph.Document.Current = PXSelect<SOOrder, Where<SOOrder.orderNbr, Equal<Required<BillingMainfestLine.orderNumber>>>>.Select(this.Base, row.OrderNumber);

                    if (graph.Document.Current != null)
                    {

                        var SOExt = graph.Document.Current.GetExtension<SOOrderExt>();
                        ext.UsrFOBValue = SOExt.UsrFOBValue;

                        foreach (SOLine item in graph.Transactions.Select())
                        {
                            InventoryItem inventory = PXSelect<InventoryItem, Where<InventoryItem.inventoryID, Equal<Required<SOLine.inventoryID>>>>.Select(this.Base, item.InventoryID);
                            // SOLine line = PXSelect<SOLine, Where<SOLine.inventoryID, Equal<Required<SOLine.inventoryID>>, And<SOLine.orderNbr,Equal<Required<SOLine.orderNbr>>>>>.Select(this.Base, row.OrderNumber);
                            if (inventory != null && inventory.InventoryCD.Trim() == "FR")
                            {
                                ext.UsrFreight = item.CuryUnitPrice;
                                break;
                            }
                        }

                    }
                }
            }
        }

        protected void BillingMainfest_RowSelected(PXCache cache, PXRowSelectedEventArgs e)
        {

            decimal? FOBTotal = 0;
            decimal? FRTotal = 0;
            var row = (BillingMainfest)e.Row;

            if (row != null)
            {
                var ext = Base.Document.Current.GetExtension<BillingMainfestExt>();
                if (ext != null)
                {
                    foreach (BillingMainfestLine item in this.Base.Transactions.Select())
                    {
                        var lineExt = item.GetExtension<BillingMainfestLineExt>();
                        FOBTotal += lineExt.UsrFOBValue;
                        FRTotal += lineExt.UsrFreight;
                    }

                    ext.UsrFreightTotal = FRTotal;
                    ext.UsrTotalFOB=FOBTotal;
                }
            }
        }

        //protected void BillingMainfest_RowInserting(PXCache cache, PXRowInsertingEventArgs e)
        //{

        //    var row = (BillingMainfest)e.Row;

        //    if (row != null)
        //    {
        //        var ext = row.GetExtension<BillingMainfestExt>();

        //        if (ext != null)
        //            if (ext.UsrStatus == status.Complete)
        //            {
        //                // Acuminator disable once PX1070 UiPresentationLogicInEventHandlers [Justification]
        //                PXUIFieldAttribute.SetEnabled<BillingMainfestLine.customerID>(cache, row, false);
        //                // Acuminator disable once PX1070 UiPresentationLogicInEventHandlers [Justification]
        //                PXUIFieldAttribute.SetEnabled<BillingMainfestLine.locationID>(cache, row, false);
        //                cache.AllowUpdate = false;

        //            }
        //            else
        //            {
        //                cache.AllowUpdate = true;
        //                Base.Transactions.Cache.AllowUpdate = true;
        //                Base.Transactions.Cache.AllowInsert = true;
        //                // Acuminator disable once PX1070 UiPresentationLogicInEventHandlers [Justification]
        //                PXUIFieldAttribute.SetEnabled<BillingMainfestLine.customerID>(cache, row, true);
        //                // Acuminator disable once PX1070 UiPresentationLogicInEventHandlers [Justification]
        //                PXUIFieldAttribute.SetEnabled<BillingMainfestLine.locationID>(cache, row, true);
        //            }
        //    }
        //}

        #endregion
        public class status
        {
            public const string Complete = "C";
        }
    }
}