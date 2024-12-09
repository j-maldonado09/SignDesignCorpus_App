// variables *******************************************************************
var cloneCount = 1;
var workOrderId;
var isNew;

// *********************************************************************************
function onEdit(e) {
    var editWindow = this.editable.element.data("kendoWindow");
    editWindow.setOptions({ height: "90%", width: "90%", resizable: false });
    editWindow.center();
    cloneCount = 1;
    alert('onEDIT');

    $(".k-grid-update").off().on("click", SaveOrUpdate);

    if (!e.model.isNew()) {
        alert('update');
        isNew = false;
        workOrderId = e.model.get('Id');
        $.ajax({
            type: 'GET',
            url: '/WorkOrder/ReadWorkOrders',
            contentType: 'application/json; charset=utf-8',
            data: { id: e.model.get("Id") },
            success: function (result) {
                LoadWorkOrderInfo(result);
            },
            error: function () {
                alert('Failed to receive the Data');
                console.log('Failed ');
            }
        })
    }
    else {
        alert('create');
        isNew = true;
    }
}

// *********************************************************************
function SaveOrUpdate() {
    if (isNew)
        SaveWorkOrder();
    else
        UpdateWorkOrder();
}

// *********************************************************************************
function LoadWorkOrderInfo(result) {
    var jsonResult = jQuery.parseJSON(result);
    var dropdownVal;

    dropdownVal = jsonResult[0].MaterialRequestedFromId;
    $("#requestedFrom").data("kendoDropDownList").value(dropdownVal);

    dropdownVal = jsonResult[0].FY;
    $("#year").data("kendoDropDownList").value(dropdownVal);

    dropdownVal = jsonResult[0].DepartmentId;
    $("#department").data("kendoDropDownList").value(dropdownVal);

    dropdownVal = jsonResult[0].AccountId;
    $("#account").data("kendoDropDownList").value(dropdownVal);

    dropdownVal = jsonResult[0].FundId;
    $("#fund").data("kendoDropDownList").value(dropdownVal);

    dropdownVal = jsonResult[0].TaskId;
    $("#task").data("kendoDropDownList").value(dropdownVal);

    dropdownVal = jsonResult[0].PCBusId;
    $("#pcBus").data("kendoDropDownList").value(dropdownVal);

    dropdownVal = jsonResult[0].ProjectId;
    $("#project").data("kendoDropDownList").value(dropdownVal);

    dropdownVal = jsonResult[0].ActivityId;
    $("#activity").data("kendoDropDownList").value(dropdownVal);

    dropdownVal = jsonResult[0].ResTypeId;
    $("#resType").data("kendoDropDownList").value(dropdownVal);

    dropdownVal = jsonResult[0].RequestedByMaintenanceId;
    $("#requestedByMaintenance").data("kendoDropDownList").value(dropdownVal);

    dropdownVal = jsonResult[0].ApprovedByMaintenanceId;
    $("#approvedByMaintenance").data("kendoDropDownList").value(dropdownVal);

    dropdownVal = jsonResult[0].ApprovedByDistrictId;
    $("#approvedByDistrict").data("kendoDropDownList").value(dropdownVal);

    var dateParts = jsonResult[0].RequestedByMaintenanceDate.split('-');
    var date = new Date(dateParts[0], dateParts[1] - 1, dateParts[2]);
    $("#requestedByMaintenanceDate").data("kendoDatePicker").value(date);

    dateParts = jsonResult[0].ApprovedByMaintenanceDate.split('-');
    date = new Date(dateParts[0], dateParts[1] - 1, dateParts[2]);
    $("#approvedByMaintenanceDate").data("kendoDatePicker").value(date);

    dateParts = jsonResult[0].ApprovedByDistrictDate.split('-');
    date = new Date(dateParts[0], dateParts[1] - 1, dateParts[2]);
    $("#approvedByDistrictDate").data("kendoDatePicker").value(date);

    alert('w');
    LoadWorkOrderItemInfo(jsonResult);
}

// *********************************************************************************
function LoadWorkOrderItemInfo(jsonResult) {
    var itemsNumber = jsonResult[0].Items.length;
    var controlVal;

    for (var i = 2; i <= itemsNumber; i++) {
        addSegment();
    }

    for (var i = 1; i <= itemsNumber; i++) {
        controlVal = jsonResult[0].Items[i - 1].NIGP;
        //$("#nigp-" + i).data("kendoDropDownList").value(controlVal);
        $("#nigp-" + i).data("kendoTextArea").value(controlVal);

        controlVal = jsonResult[0].Items[i - 1].Quantity;
        $("#quantity-" + i).data("kendoNumericTextBox").value(controlVal);

        controlVal = jsonResult[0].Items[i - 1].SignImage;
        alert(controlVal);
        //var item = [{name: controlVal, size: 0, extension: ""}];
        var item = [];
        item.push(controlVal);
        alert(item);
        $("#files-" + i).data("kendoUpload").files = item;
        alert($("#files-" + i).data("kendoUpload").files);

        controlVal = jsonResult[0].Items[i - 1].Latitude;
        $("#latitude-" + i).data("kendoTextArea").value(controlVal);

        controlVal = jsonResult[0].Items[i - 1].Longitude;
        $("#longitude-" + i).data("kendoTextArea").value(controlVal);

        controlVal = jsonResult[0].Items[i - 1].Instructions;
        $("#instructions-" + i).data("kendoTextArea").value(controlVal);

        controlVal = jsonResult[0].Items[i - 1].SpecialInstructions;
        $("#specialInstructions-" + i).data("kendoTextArea").value(controlVal);
    }
}

// *********************************************************************************
function ProjectNameEditor(container, options) {
    $('<input required name="Projects">')
        .appendTo(container)
        .kendoDropDownList({
            autoBind: false,
            dataTextField: "Name",
            dataValueField: "Id",
            dataSource: {
                type: "ajax",
                transport: {
                    read: "WorkOrder/GetProjects"
                }
            }
        });
}

// *********************************************************************************
function MaintenanceSectionContactNameEditor(container, options) {
    $('<input required name="MaintenanceSectionContacts">')
        .appendTo(container)
        .kendoDropDownList({
            autoBind: false,
            dataTextField: "FullName",
            dataValueField: "Id",
            dataSource: {
                type: "ajax",
                transport: {
                    read: "WorkOrder/GetMaintenanceSectionContacts"
                }
            }
        });
}

// *********************************************************************************
function DistrictContactNameEditor(container, options) {
    $('<input required name="DistrictContacts">')
        .appendTo(container)
        .kendoDropDownList({
            autoBind: false,
            dataTextField: "FullName",
            dataValueField: "Id",
            dataSource: {
                type: "ajax",
                transport: {
                    read: "WorkOrder/GetDistrictContacts"
                }
            }
        });
}

// Function to select latitude and longitude by clicking on the map
function onClick(e) {
    var latitude = $("#latitude").data("kendoTextArea");
    var longitude = $("#longitude").data("kendoTextArea");
    var location = e.location;

    latitude.value(location.lat);
    latitude.trigger("change");

    longitude.value(location.lng);
    longitude.trigger("change");
}

// Function to add a block for a new sign
function addSegment() {
    cloneCount++;
    $('#sign-container').append('<div id="sign-form-' + cloneCount + '"></div>');
    $('#sign-form-' + cloneCount).append('<hr/>');
    $('#sign-form-' + cloneCount).append('<div id="padding-y-' + cloneCount + '" class="row py-3"></div>');
    $('#padding-y-' + cloneCount).append('<div id="left-' + cloneCount + '" class="form-group col-12 col-sm-12 col-md-6 col-lg-6 col-xl-6"></div>');
    $('#left-' + cloneCount).append('<div id="first-row-' + cloneCount + '" class="row pt-3"></div>');
    $('#first-row-' + cloneCount).append('<div id="first-row-left-' + cloneCount + '" class="form-group col-12 col-sm-12 col-md-6 col-lg-6 col-xl-6"></div>');
    $('#first-row-left-' + cloneCount).append('<h6 class="pt-3 align-text-bottom">NIGP :</h6>');

    //createDropdown();
    createTextArea('#first-row-left-' + cloneCount, 'nigp-' + cloneCount, 1);

    $('#first-row-' + cloneCount).append('<div id="first-row-right-' + cloneCount + '" class="form-group col-12 col-sm-12 col-md-6 col-lg-6 col-xl-6"></div>');
    $('#first-row-right-' + cloneCount).append('<h6 class="pt-3">Quantity:</h6>');

    createNumericTextbox();

    $('#left-' + cloneCount).append('<h6 class="align-text-bottom, pt-3">Image Upload</h6>');

    createUpload();

    $('#left-' + cloneCount).append('<div id="second-row-' + cloneCount + '" class="row pt-3"></div>');
    $('#second-row-' + cloneCount).append('<div id="second-row-left-' + cloneCount + '" class="form-group col-12 col-sm-12 col-md-6 col-lg-6 col-xl-6"></div>');
    $('#second-row-left-' + cloneCount).append('<h6 class="pt-3">Latitude:</h6>');

    createTextArea('second-row-left-' + cloneCount, 'latitude-' + cloneCount, 1);

    $('#second-row-' + cloneCount).append('<div id="second-row-right-' + cloneCount + '" class="form-group col-12 col-sm-12 col-md-6 col-lg-6 col-xl-6"></div>');
    $('#second-row-right-' + cloneCount).append('<h6 class="pt-3">Longitude:</h6>');

    createTextArea('second-row-right-' + cloneCount, 'longitude-' + cloneCount, 1);

    $('#left-' + cloneCount).append('<h6 class="pt-3">Instructions:</h6>');

    createTextArea('left-' + cloneCount, 'instructions-' + cloneCount, 2);

    $('#left-' + cloneCount).append('<h6 class="pt-3">Special Instructions:</h6>');

    createTextArea('left-' + cloneCount, 'specialInstructions-' + cloneCount, 3);

    $('#padding-y-' + cloneCount).append('<div id="right-' + cloneCount + '" class="form-group col-12 col-sm-12 col-md-6 col-lg-6 col-xl-6"></div>');

    createMap();
}

// Function to create a kendo dropdown list dynamically
function createDropdown() {
    $('#first-row-left-' + cloneCount).append('<input id="nigp-' + cloneCount + '">');
    $('#nigp-' + cloneCount).kendoDropDownList({
        dataTextField: 'Name',
        dataValueField: 'Id',
        filter: 'startsWith',
        dataSource: {
            transport: {
                read: {
                    url: 'WorkOrder/GetNIGPsAux',
                    dataType: 'json'
                }
            }
        },
        value: null
    });
}

// Function to create kendo numeric texbox dynamically
function createNumericTextbox() {
    $('#first-row-right-' + cloneCount).append('<input id="quantity-' + cloneCount + '" />');
    $('#quantity-' + cloneCount).kendoNumericTextBox({
        value: 1,
        format: 'n0',
        min: 1,
        max: 10,
    });
}

// Function to create kendo text area dynamically
function createTextArea(idToAppend, id, rowsNumber) {
    $('#' + idToAppend).append('<textarea id="' + id + '" style="width: 100%;"></textarea>');
    $('#' + id).kendoTextArea({
        rows: rowsNumber
    });
}

// Function to create a kendo upload dynamically
function createUpload() {
    $('#left-' + cloneCount).append('<input id="files-' + cloneCount + '" name="files" type="file"/>');
    $('#files-' + cloneCount).kendoUpload({
        multiple: false,
        async: {
            saveUrl: '/WorkOrder/SaveUploadedImage',
            removeUrl: "remove",
            autoUpload: true
        }
    });
}

// Function to create a kendo map dynamically
function createMap() {
    var mapNum = cloneCount;

    $('#right-' + cloneCount).append('<div id="map-' + cloneCount + '" style="width: 100%; height: 510px;"></div>');
    $('#map-' + cloneCount).kendoMap({
        zoom: 11,
        center: [27.8006, -97.3964],
        layers: [{
            type: 'tile',
            urlTemplate: 'http://a.tile.openstreetmap.org/#= zoom #/#= x #/#= y #.png',
            attribution: '&copy; <a href="http://osm.org/copyright">OpenStreetMap contributors</a>.'
        }],
        click: function (e) {
            var latitude = $('#latitude-' + mapNum).data("kendoTextArea");
            var longitude = $('#longitude-' + mapNum).data("kendoTextArea");
            var location = e.location;

            latitude.value(location.lat);
            latitude.trigger("change");

            longitude.value(location.lng);
            longitude.trigger("change");
        }
    });
}

// Function to save work order
function SaveWorkOrder() {
    var workOrder = GetWorkOrderData();
    //alert(JSON.stringify(workOrder));
    $.ajax({
        type: 'POST',
        url: '/WorkOrder/Create',
        contentType: 'application/json; charset=utf-8',
        data: JSON.stringify(workOrder),
        success: function (result) {
            alert('Successfully received Data ');
            console.log(result);
        },
        error: function () {
            alert('Failed to receive the Data');
            console.log('Failed ');
        }
    });
}

// Function to edit/update work order
function UpdateWorkOrder() {
    var workOrder = GetWorkOrderData();
    $.ajax({
        type: 'POST',
        url: '/WorkOrder/Update',
        contentType: 'application/json; charset=utf-8',
        data: JSON.stringify(workOrder),
        success: function (result) {
            alert('Successfully received Data ');
            console.log(result);
        },
        error: function () {
            alert('Failed to receive the Data');
            console.log('Failed ');
        }
    });
}

// ************************************************
function CreatePDF() {
    var workOrder = GetWorkOrderData();
    $.ajax({
        type: 'POST',
        url: '/WorkOrder/ExportPdf',
        contentType: 'application/json; charset=utf-8',
        data: JSON.stringify(workOrder),
        success: function (result) {
            //alert('Successfully received Data ');
            console.log(result);
        },
        error: function () {
            alert('Failed to receive the Data');
            console.log('Failed ');
        }
    });
}

// *********************************************************************************
function GetWorkOrderData() {
    var files = [];

    var workOrder = {
        Id: workOrderId,
        Number: 'a-1',
        MaterialRequestedFromId: $("#requestedFrom").val(),
        MaterialRequestedById: 2,
        DepartmentId: $("#department").val(),
        AccountId: $("#account").val(),
        FY: $("#year").val(),
        FundId: $("#fund").val(),
        TaskId: $("#task").val(),
        PCBusId: $("#pcBus").val(),
        ProjectId: $("#project").val(),
        ActivityId: $("#activity").val(),
        ResTypeId: $("#resType").val(),
        RequestedByMaintenanceId: $("#requestedByMaintenance").val(),
        RequestedByMaintenanceDate: new Date($("#requestedByMaintenanceDate").val()),
        ApprovedByMaintenanceId: $("#approvedByMaintenance").val(),
        ApprovedByMaintenanceDate: new Date($("#approvedByMaintenanceDate").val()),
        ApprovedByDistrictId: $("#approvedByDistrict").val(),
        ApprovedByDistrictDate: new Date($("#approvedByDistrictDate").val()),
        Items: []
    }

    for (var i = 1; i <= cloneCount; i++) {
        if ($('#sign-form-' + i).length > 0) {
            files = $("#files-" + i).data("kendoUpload").getFiles();
            workOrder.Items.push({
                NIGP: $("#nigp-" + i).val(),
                Quantity: $("#quantity-" + i).val(),
                Instructions: $("#instructions-" + i).val(),
                SignImage: files[0].name,
                Latitude: $("#latitude-" + i).val(),
                Longitude: $("#longitude-" + i).val(),
                SpecialInstructions: $("#specialInstructions-" + i).val()
            });
        }
    }
    alert(JSON.stringify(workOrder));
    return workOrder;
}