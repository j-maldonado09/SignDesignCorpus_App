using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using SignDesignCorpus.HelperModels;
using Telerik.Documents.Core;
using Telerik.Documents.Fixed;
using Telerik.Zip;

namespace SignDesign_App.Services
{
    public class ExportPdf
    {
        public void CreatePdf(WorkOrderHelperModel workOrder, WorkOrderNamesHelperModel workOrderNamesHelperModel, string physicalPath)
        {
            string fileName = Path.Combine(physicalPath, "SignRequestPDFs", "SignRequest_" + workOrderNamesHelperModel.MaterialRequestedByNumber + "-" + workOrder.Id + ".pdf");
            float narrowBorder = 0.5f;
            float thickBorder = 3f;
            string fieldColor = "#FFFFFF";
            string shopColor = "BFFFF6";
            string rushText = "";
            string newText = "";

            Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(2, Unit.Centimetre);

                    page.Header()
                        .Column(column =>
                        {
                            column.Item()
                                .Row(row =>
                                {
                                    row.ConstantItem(75)
                                       .Image(Path.Combine(physicalPath, "images", "logo form 1593.png"));
                                    row.RelativeItem()
                                        .AlignRight()
                                        .Text("Work Order #: ")
                                        .FontSize(11);
                                    row.ConstantItem(85)
                                        .Text(" " + workOrderNamesHelperModel.MaterialRequestedByNumber + "-" + workOrder.Id + " ")
                                        .FontSize(11)
                                        .BackgroundColor(fieldColor);
                                });
                            column.Item()
                                .PaddingLeft(4)
                                .Text(text =>
                                {
                                    text.Span("Page ").FontSize(8);
                                    text.CurrentPageNumber().FontSize(8);
                                    text.Span(" of ").FontSize(8);
                                    text.TotalPages().FontSize(8);
                                });
                        });


                    page.Content()

                        .Column(col =>
                        {
                            col.Item()
                                .Row(row =>
                                {
                                    row.ConstantItem(162)
                                        .Column(col =>
                                        {
                                            col.Item()
                                                .Text(" REGIONAL SIGN REQUEST ")
                                                .Bold().FontSize(13);
                                            col.Item()
                                                .PaddingTop(25)
                                                .BorderTop(narrowBorder)
                                                .BorderVertical(narrowBorder)
                                                .Text(" Material Requested From ")
                                                .FontSize(11);
                                            col.Item()
                                                .BorderBottom(narrowBorder)
                                                .BorderVertical(narrowBorder)
                                                .Background(fieldColor)
                                                .Text(" " + workOrderNamesHelperModel.MaterialRequestedFromName)
                                                .FontSize(11);
                                        });
                                    row.Spacing(2);
                                    row.ConstantItem(100)
                                       .Column(col =>
                                       {
                                           col.Item()
                                              .PaddingTop(62)
                                              .BorderTop(narrowBorder)
                                              .BorderVertical(narrowBorder)
                                              .Text(" Dest. INBU ") //Dest. INBU field
                                              .FontSize(11);
                                           col.Item()
                                              .BorderBottom(narrowBorder)
                                              .BorderVertical(narrowBorder)
                                              .Background(fieldColor)
                                              .Text(" ")
                                              .FontSize(11);
                                       });
                                    row.RelativeItem()
                                        .AlignRight()
                                        .Column(col =>
                                        {
                                            col.Item()
                                                .BorderTop(narrowBorder)
                                                .BorderVertical(narrowBorder)
                                                .Text("  Material Requested By  ")
                                                .FontSize(11);
                                            col.Item()
                                                .BorderVertical(narrowBorder)
                                                .Text("  (i.e. Name of Requestor, Name of City/Town, and Contact Phone/Email)  ")
                                                .FontSize(7)
                                                .FontColor("#B7B7B7");
                                            col.Item()
                                                .BorderVertical(narrowBorder)
                                                .Background(fieldColor)
                                                .Text(" " + workOrderNamesHelperModel.MaterialRequestedByName + " - " + workOrderNamesHelperModel.MaterialRequestedByNumber)
                                                .FontSize(10);
                                            col.Item()
                                                .BorderVertical(narrowBorder)
                                                .Background(fieldColor)
                                                .Text(" " + workOrderNamesHelperModel.MaterialRequestedByAddress)
                                                .FontSize(10);
                                            col.Item()
                                                .BorderVertical(narrowBorder)
                                                .Background(fieldColor)
                                                .Text(" " + workOrderNamesHelperModel.MaterialRequestedByCity + ", " + workOrderNamesHelperModel.MaterialRequestedByState + " " + workOrderNamesHelperModel.MaterialRequestedByZipCode)
                                                .FontSize(10);
                                            col.Item()
                                                .BorderBottom(narrowBorder)
                                                .BorderVertical(narrowBorder)
                                                .Background(fieldColor)
                                                .Text(" " + workOrderNamesHelperModel.MaterialRequestedByEmail.ToLower())
                                                .FontSize(10);
                                        });
                                });
                            col.Item()
                                .PaddingTop(10)
                                .Table(table => {
                                    table.ColumnsDefinition(columns =>
                                    {
                                        columns.RelativeColumn();
                                        columns.RelativeColumn();
                                        columns.RelativeColumn();
                                        columns.RelativeColumn();
                                        columns.RelativeColumn();
                                        columns.RelativeColumn();
                                        columns.RelativeColumn(2);
                                        columns.RelativeColumn();
                                        columns.RelativeColumn();
                                    });
                                    table.Cell().Row(1).Column(1).Border(narrowBorder).BorderBottom(thickBorder).AlignCenter().Text("Dept").SemiBold().FontSize(9);
                                    table.Cell().Row(1).Column(2).Border(narrowBorder).BorderBottom(thickBorder).AlignCenter().Text("Account").SemiBold().FontSize(9);
                                    table.Cell().Row(1).Column(3).Border(narrowBorder).BorderBottom(thickBorder).AlignCenter().Text("FY").SemiBold().FontSize(9);
                                    table.Cell().Row(1).Column(4).Border(narrowBorder).BorderBottom(thickBorder).AlignCenter().Text("Fund").SemiBold().FontSize(9);
                                    table.Cell().Row(1).Column(5).Border(narrowBorder).BorderBottom(thickBorder).AlignCenter().Text("Task").SemiBold().FontSize(9);
                                    table.Cell().Row(1).Column(6).Border(narrowBorder).BorderBottom(thickBorder).AlignCenter().Text("PC Bus").SemiBold().FontSize(9);
                                    table.Cell().Row(1).Column(7).Border(narrowBorder).BorderBottom(thickBorder).AlignCenter().Text("Project").SemiBold().FontSize(9);
                                    table.Cell().Row(1).Column(8).Border(narrowBorder).BorderBottom(thickBorder).AlignCenter().Text("Activity").SemiBold().FontSize(9);
                                    table.Cell().Row(1).Column(9).Border(narrowBorder).BorderBottom(thickBorder).AlignCenter().Text("Res. Type").SemiBold().FontSize(9);

                                    table.Cell().Row(2).Column(1).Background(fieldColor).Border(narrowBorder).AlignCenter().Text(workOrderNamesHelperModel.DepartmentName).SemiBold().FontSize(9);
                                    table.Cell().Row(2).Column(2).Background(fieldColor).Border(narrowBorder).AlignCenter().Text(workOrderNamesHelperModel.AccountName).SemiBold().FontSize(9);
                                    table.Cell().Row(2).Column(3).Background(fieldColor).Border(narrowBorder).AlignCenter().Text(workOrder.FY).FontSize(9);
                                    table.Cell().Row(2).Column(4).Background(fieldColor).Border(narrowBorder).AlignCenter().Text(workOrderNamesHelperModel.FundName).SemiBold().FontSize(9);
                                    table.Cell().Row(2).Column(5).Background(fieldColor).Border(narrowBorder).AlignCenter().Text(workOrderNamesHelperModel.TaskName).SemiBold().FontSize(9);
                                    table.Cell().Row(2).Column(6).Background(fieldColor).Border(narrowBorder).AlignCenter().Text(workOrderNamesHelperModel.PCBusName).SemiBold().FontSize(9);
                                    table.Cell().Row(2).Column(7).Background(fieldColor).Border(narrowBorder).AlignCenter().Text(workOrderNamesHelperModel.ProjectName).SemiBold().FontSize(9);
                                    table.Cell().Row(2).Column(8).Background(fieldColor).Border(narrowBorder).AlignCenter().Text(workOrderNamesHelperModel.ActivityName).SemiBold().FontSize(9);
                                    table.Cell().Row(2).Column(9).Background(fieldColor).Border(narrowBorder).AlignCenter().Text(workOrderNamesHelperModel.ResTypeName).SemiBold().FontSize(9);
                                });

                            col.Item()
                                .PaddingTop(10)
                                .Row(row =>
                                {
                                    row.RelativeItem(2f)
                                        .Column(col => {
                                            col.Item()
                                                .Padding(2)
                                                .Text("Requested By:")
                                                .FontSize(12);
                                            col.Item()
                                                .Text("")
                                                .FontSize(8);
                                            col.Item()
                                                .PaddingLeft(2)
                                                .Text("Approved By:")
                                                .FontSize(12);
                                            col.Item()
                                                .Text("")
                                                .FontSize(8);
                                            col.Item()
                                                .PaddingLeft(2)
                                                .Text("Approved By:")
                                                .FontSize(12);
                                            col.Item()
                                                .Text("")
                                                .FontSize(8);
                                        });
                                    row.RelativeItem(4)
                                        .Column(col => {
                                            col.Item()
                                                .PaddingTop(2)
                                                .Background(fieldColor)
                                                .Text(" " + workOrderNamesHelperModel.RequestedByMaintenanceName)
                                                .FontSize(11);
                                            col.Item()
                                                .Text("")
                                                .FontSize(8);
                                            col.Item()
                                                .PaddingTop(2)
                                                .Background(fieldColor)
                                                .Text(" " + workOrderNamesHelperModel.ApprovedByMaintenanceName)
                                                .FontSize(11);
                                            col.Item()
                                                .Text("Maintenance Supervisor")
                                                .FontSize(8);
                                            col.Item()
                                                .PaddingTop(2)
                                                .Background(fieldColor)
                                                .Text(" " + workOrderNamesHelperModel.ApprovedByDistrictName)
                                                .FontSize(11);
                                            col.Item()
                                                .Text("District Traffic Office")
                                                .FontSize(8);

                                        });
                                    row.RelativeItem()
                                        .Column(col => {
                                            col.Item()
                                                .PaddingTop(2)
                                                .PaddingLeft(8)
                                                .Text("Date:")
                                                .FontSize(12);
                                            col.Item()
                                                .Text("")
                                                .FontSize(8);
                                            col.Item()
                                               .PaddingTop(2)
                                               .PaddingLeft(8)
                                               .Text("Date:")
                                               .FontSize(12);
                                            col.Item()
                                                .Text("")
                                                .FontSize(8);
                                            col.Item()
                                               .PaddingTop(2)
                                               .PaddingLeft(8)
                                               .Text("Date:")
                                               .FontSize(12);
                                            col.Item()
                                                .Text("")
                                                .FontSize(8);

                                        });
                                    row.RelativeItem(2)
                                        .Column(col => {
                                            col.Item()
                                                .PaddingTop(2)
                                                .PaddingRight(5)
                                                .Background(fieldColor)
                                                .Text(" " + workOrder.RequestedByMaintenanceDate.ToString("MM/dd/yyyy"))
                                                .FontSize(12);
                                            col.Item()
                                                .Text("")
                                                .FontSize(8);
                                            col.Item()
                                                .PaddingTop(2)
                                                .PaddingRight(5)
                                                .Background(fieldColor)
                                                .Text(" " + workOrder.ApprovedByMaintenanceDate?.ToString("MM/dd/yyyy"))
                                                .FontSize(12);
                                            col.Item()
                                                .Text("")
                                                .FontSize(8);
                                            col.Item()
                                                .PaddingTop(2)
                                                .PaddingRight(5)
                                                .Background(fieldColor)
                                                .Text(" " + workOrder.ApprovedByDistrictDate?.ToString("MM/dd/yyyy"))
                                                .FontSize(12);
                                            col.Item()
                                                .Text("")
                                                .FontSize(8);

                                        });
                                    row.RelativeItem(3)
                                        .Border(narrowBorder)
                                        .Background(shopColor)
                                        .Column(col => {
                                            col.Item()
                                                .PaddingTop(4)
                                                .PaddingRight(5)
                                                .AlignCenter()
                                                .Text("Send to Attention:")
                                                .FontSize(10);
                                            col.Item()
                                                .AlignCenter()
                                                .Text("Lubbock R.S.S.")
                                                .FontSize(9);
                                            col.Item()
                                                .AlignCenter()
                                                .Text("Lubbock, Texas 75751")
                                                .FontSize(9);
                                            col.Item()
                                                 .AlignCenter()
                                                 .Text("Phone: 806-748-4408")
                                                 .FontSize(9);
                                            col.Item()
                                                .AlignCenter()
                                                .Text("Email: RCW-Sign-Shop@txdot.gov")
                                                .FontSize(9);
                                            col.Item()
                                                 .AlignCenter()
                                                 .Text("")
                                                 .FontSize(7);
                                        });
                                });

                            for (int i = 0; i < workOrder.Items.Count; i++)
                            {
                                rushText = workOrder.Items[i].Rush ? "; RUSH ORDER" : "";
                                newText = workOrder.Items[i].New ? "; New Installation" : "";

                                col.Item()
                                .PaddingTop(20)
                                .Row(row =>
                                {
                                    row.RelativeItem(1.1f)
                                        .BorderLeft(narrowBorder)
                                        .BorderHorizontal(narrowBorder)
                                        .Column(col => {
                                            col.Item()
                                                .Padding(2)
                                                .Text("NIGP #:")
                                                .FontSize(11);
                                            col.Item()
                                                .PaddingLeft(2)
                                                .Text("Qty.:")
                                                .FontSize(11);
                                            col.Item()
                                                .Padding(2)
                                                .Text(" ")
                                                .FontSize(11);

                                        });
                                    row.RelativeItem(2)
                                        .BorderHorizontal(narrowBorder)
                                        .BorderRight(narrowBorder)
                                        .Column(col => {
                                            col.Item()
                                                .PaddingTop(2)
                                                .PaddingRight(2)
                                                .Background(fieldColor)
                                                .Text(" " + workOrder.Items[i].NIGP)
                                                .FontSize(11);
                                            col.Item()
                                                .PaddingTop(2)
                                                .PaddingRight(2)
                                                .Background(fieldColor)
                                                .Text("  " + workOrder.Items[i].Quantity)
                                                .FontSize(11);
                                            //col.Item()
                                            //    .PaddingHorizontal(2)
                                            //    .PaddingRight(2)
                                            //    .Text("")
                                            //    .FontSize(12);

                                        });
                                    row.RelativeItem(9)
                                        .BorderHorizontal(narrowBorder)
                                        .BorderRight(narrowBorder)
                                        .Column(col => {
                                            col.Item()
                                                .Row(row =>
                                                {
                                                    row.ConstantItem(275)
                                                    .Padding(10)
                                                    .Image(Path.Combine(physicalPath, "images", "signs", workOrder.Items[i].SignImage));
                                                });
                                            //col.Item()
                                            //    .Height(55)
                                            //    .BorderBottom(narrowBorder);
                                            col.Item()
                                                .Padding(2)
                                                .Background(fieldColor)
                                                .Text("  " + workOrder.Items[i].Name + "; " + workOrder.Items[i].Size + "; " + workOrder.Items[i].Color + rushText + newText)
                                                .FontSize(11);
                                        });
                                });
                                col.Item()
                                    .Height(70)
                                    .Border(narrowBorder)
                                    .PaddingHorizontal(2)
                                    .Background(fieldColor)
                                    .Text(text =>
                                    {
                                        text.Span("Special Instructions\n").FontSize(12);
                                        text.Span(workOrder.Items[i].SpecialInstructions + "\nLocation: ("
                                                  + workOrder.Items[i].Latitude + ", "
                                                  + workOrder.Items[i].Longitude + ").").FontSize(11);
                                    });
                                //col.Item()
                                //    .Border(narrowBorder)
                                //    .PaddingHorizontal(2)
                                //    .Text("Location")
                                //    .FontSize(12);
                                //col.Item()
                                //    .Border(narrowBorder)
                                //    .Background(fieldColor)
                                //    .Text("")
                                //    .FontSize(12);
                            }
                        });

                    page.Footer();

                });
            })
            .GeneratePdf(fileName);

            //.GeneratePdf(Path.Combine(physicalPath, "SignRequestPDFs", "SignRequest_" + workOrder.Id + ".pdf"));

            //Process.Start("explorer.exe", "SignRequest.pdf");
            //Process.Start(Path.Combine(physicalPath, "SignRequest.pdf"));
        }
    }
}
