$(document).on("click", ".Index", function ()
{
    location.reload();
});

$(document).ready(function () {
    if ($('#hdnCurrentPage').val().length > 0)
    {
        //alert($('#hdnCurrentPage').val());
        $.get($('#hdnCurrentPage').val(), function (data) {
            $('#page-wrapper').html(data);
            LoadLeftMenu();
        });
    }

    $.ajaxSetup({
        beforeSend: function (xhr) { $('#pageloader-overlay').attr('style', 'display:'); },
        complete: function (xhr, status) { $('#pageloader-overlay').attr('style', 'display:none'); }
    });
});

function LoadLeftMenu()
{
    $.get("/Home/LeftMenu", function (data) {
        $('#sidebar-wrapper1').html(data);
        $.sidebarMenu($('.sidebar-menu'));
    });
}

LoadLeftMenu();
// ****************************************** Admin Module - Begin ***************************************
$(document).on("click", ".AdminIndex", function () {

    $.get("/Home/AdminIndex", function (data) {
        $('#page-wrapper').html(data);
        LoadLeftMenu();
    });
});

$(document).on("click", ".CompanyIndex", function () {

    $.get("/Home/CompanyIndex", function (data) {
        $('#page-wrapper').html(data);
        LoadLeftMenu();
    });
});

function CreateCompany() {
    $.get("Home/CompanyCreate", function (data) {
        $('#page-wrapper').html(data);
    });
}

function EditCompany(id) {
    var url = "Home/CompanyEdit/" + id;
    $.get(url, function (data) {
        $('#page-wrapper').html(data);
    });
}

$(document).on("click", ".UserIndex", function () {
    $.get("/User/Index", function (data)
    {
        $('#page-wrapper').html(data);
        LoadLeftMenu();
    });
});

function CreateUser() {
    $.get("User/Create", function (data) {
        $('#page-wrapper').html(data);
    });
}

function EditUser(id) {
    var url = "User/Edit/" + id;
    $.get(url, function (data) {
        $('#page-wrapper').html(data);
    });
}

$(document).on("click", ".ChangePassword", function () {

    $.get("/User/ChangePassword", function (data) {
        $('#page-wrapper').html(data);
        LoadLeftMenu();
    });
});

// ****************************************** Admin Module - End ***************************************

// ****************************************** Assessment Module - Begin ***************************************
$(document).on("click", ".AssessmentIndex", function () {
    
    $.get("/Assessment/Index", function (data) {
        $('#page-wrapper').html(data);
        LoadLeftMenu();
    });
});

$(document).on("click", ".AssessmentsIndex", function () {

    $.get("/Assessment/AssessmentIndex", function (data) {
        $('#page-wrapper').html(data);
        LoadLeftMenu();
    });
});

function ViewAssessmentIFReport(id) {
    var url = "/Assessment/AssessmentInternalFinishesReport?ID=" + id;
    $.get(url, function (data) {
        $('#page-wrapper').html(data);
    });
}

function ViewAssessmentReport(id) {
    var url = "/Assessment/AssessmentReport?ID=" + id;
    $.get(url, function (data) {
        $('#page-wrapper').html(data);
    });
}

// /////// Assessors Module - Begin////////////////////////////////
$(document).on("click", ".AssessorIndex", function () {
    $.get("/Assessment/AssessorIndex", function (data) {
        $('#page-wrapper').html(data);
        LoadLeftMenu();
    });
});

function CreateAssessor() {
    $.get("Assessment/AssessorCreate", function (data) {
        $('#page-wrapper').html(data);
    });
}

function EditAssessor(id) {
    var url = "Assessment/AssessorEdit/" + id;
    $.get(url, function (data) {
        $('#page-wrapper').html(data);
    });
}
// /////// Assessors Module - End////////////////////////////////

// /////// Assessment Project Module - Begin////////////////////////////////
$(document).on("click", ".AssessmentProjectIndex", function () {
    $.get("/Assessment/ProjectIndex", function (data) {
        $('#page-wrapper').html(data);
    });
});

function CreateAssessmentProject() {
    $.get("Assessment/ProjectCreate", function (data) {
        $('#page-wrapper').html(data);
    });
}

function EditAssessmentProject(id) {
    var url = "Assessment/ProjectEdit/" + id;
    $.get(url, function (data) {
        $('#page-wrapper').html(data);
    });
}
// /////// Assessment Project Module - End////////////////////////////////

// /////// Module Module - Begin////////////////////////////////
$(document).on("click", ".ModuleIndex", function () {
    $.get("/Assessment/ModuleIndex", function (data) {
        $('#page-wrapper').html(data);
        LoadLeftMenu();
    });
});

function CreateModule() {
    $.get("Assessment/ModuleCreate", function (data) {
        $('#page-wrapper').html(data);
    });
}

function EditModule(id) {
    var url = "Assessment/ModuleEdit/" + id;
    $.get(url, function (data) {
        $('#page-wrapper').html(data);
    });
}
// /////// Module Module - End////////////////////////////////

// /////// Location Module - Begin////////////////////////////////
$(document).on("click", ".LocationIndex", function () {
    $.get("/Assessment/LocationIndex", function (data) {
        $('#page-wrapper').html(data);
        LoadLeftMenu();
    });
});

function CreateLocation() {
    $.get("Assessment/LocationCreate", function (data) {
        $('#page-wrapper').html(data);
    });
}

function EditLocation(id) {
    var url = "Assessment/LocationEdit/" + id;
    $.get(url, function (data) {
        $('#page-wrapper').html(data);
    });
}
// /////// Location Module - End////////////////////////////////

// /////// Module Process Module - Begin////////////////////////////////
$(document).on("click", ".ModuleProcessIndex", function () {
    $.get("/Assessment/ModuleProcessIndex", function (data) {
        $('#page-wrapper').html(data);
        LoadLeftMenu();
    });
});

function CreateModuleProcess() {
    $.get("Assessment/ModuleProcessCreate", function (data) {
        $('#page-wrapper').html(data);
    });
}

function EditModuleProcess(id) {
    var url = "Assessment/ModuleProcessEdit/" + id;
    $.get(url, function (data) {
        $('#page-wrapper').html(data);
    });
}
// /////// Module Process Module - End////////////////////////////////

// /////// Joint Module - Begin////////////////////////////////
$(document).on("click", ".JointIndex", function () {
    $.get("/Assessment/JointIndex", function (data) {
        $('#page-wrapper').html(data);
        LoadLeftMenu();
    });
});

function CreateJoint() {
    $.get("Assessment/JointCreate", function (data) {
        $('#page-wrapper').html(data);
    });
}

function EditJoint(id) {
    var url = "Assessment/JointEdit/" + id;
    $.get(url, function (data) {
        $('#page-wrapper').html(data);
    });
}
// /////// Joint Module - End////////////////////////////////

// /////// Leak Module - Begin////////////////////////////////
$(document).on("click", ".LeakIndex", function () {
    $.get("/Assessment/LeakIndex", function (data) {
        $('#page-wrapper').html(data);
        LoadLeftMenu();
    });
});

function CreateLeak() {
    $.get("Assessment/LeakCreate", function (data) {
        $('#page-wrapper').html(data);
    });
}

function EditLeak(id) {
    var url = "Assessment/LeakEdit/" + id;
    $.get(url, function (data) {
        $('#page-wrapper').html(data);
    });
}
// /////// Leak Module - End////////////////////////////////

// /////// Wall Module - Begin////////////////////////////////
$(document).on("click", ".WallIndex", function () {
    $.get("/Assessment/WallIndex", function (data) {
        $('#page-wrapper').html(data);
        LoadLeftMenu();
    });
});

function CreateWall() {
    $.get("Assessment/WallCreate", function (data) {
        $('#page-wrapper').html(data);
    });
}

function EditWall(id) {
    var url = "Assessment/WallEdit/" + id;
    $.get(url, function (data) {
        $('#page-wrapper').html(data);
    });
}
// /////// Wall Module - End////////////////////////////////

// /////// Window Module - Begin////////////////////////////////
$(document).on("click", ".WindowIndex", function () {
    $.get("/Assessment/WindowIndex", function (data) {
        $('#page-wrapper').html(data);
        LoadLeftMenu();
    });
});

function CreateWindow() {
    $.get("Assessment/WindowCreate", function (data) {
        $('#page-wrapper').html(data);
    });
}

function EditWindow(id) {
    var url = "Assessment/WindowEdit/" + id;
    $.get(url, function (data) {
        $('#page-wrapper').html(data);
    });
}
// /////// Window Module - End////////////////////////////////

function ViewAssessmentSummary(id) {
    var url = "/Assessment/AssessmentSummary?ID=" + id;
    $.get(url, function (data) {
        $('#page-wrapper').html(data);
    });
}

function EditInternalFinishes(id) {
    var url = "/Assessment/InternalFinishes?ID=" + id;
    $.get(url, function (data) {
        $('#page-wrapper').html(data);
    });
}

function EditExternalWall(id) {
    var url = "/Assessment/ExternalWall?ID=" + id;
    $.get(url, function (data) {
        $('#page-wrapper').html(data);
    });
}

function EditExternalWorks(id) {
    var url = "/Assessment/ExternalWorks?ID=" + id;
    $.get(url, function (data) {
        $('#page-wrapper').html(data);
    });
}

function EditRoofConstruction(id) {
    var url = "/Assessment/RoofConstruction?ID=" + id;
    $.get(url, function (data) {
        $('#page-wrapper').html(data);
    });
}

function EditFieldWindowWaterTightnessTest(id) {
    var url = "/Assessment/FieldWindowWTT?ID=" + id;
    $.get(url, function (data) {
        $('#page-wrapper').html(data);
    });
}

function EditWetAreaWaterTightnessTest(id) {
    var url = "/Assessment/WetAreaWTT?ID=" + id;
    $.get(url, function (data) {
        $('#page-wrapper').html(data);
    });
}

// ****************************************** Assessment Module - End ***************************************

// ****************************************** QC Inspection Module - Begin ***************************************


$(document).on("click", ".QCInspectionIndex", function () {
    $('#pageloader-overlay').attr('style', 'display:');
    $.get("/QCInspection/Index", function (data) {
        $('#page-wrapper').html(data);
        LoadLeftMenu();
    });
    $('#pageloader-overlay').attr('style', 'display:none');
});

$(document).on("click", ".PMIndex", function () {
    $('#pageloader-overlay').attr('style', 'display:');
    $.get("/QCInspection/PMIndex", function (data) {
        $('#page-wrapper').html(data);
        LoadLeftMenu();
    });
    $('#pageloader-overlay').attr('style', 'display:none');
});

$(document).on("click", ".ProjectDocumentsIndex", function () {
    $('#pageloader-overlay').attr('style', 'display:');
    $.get("/QCInspection/ProjectDocumentsIndex", function (data) {
        $('#page-wrapper').html(data);
        LoadLeftMenu();
    });
    $('#pageloader-overlay').attr('style', 'display:none');
});

$(document).on("click", ".RWFIProgressReport", function () {
    $.get("/QCInspection/RFWIProgressReportIndex", function (data) {
        $('#page-wrapper').html(data);
        LoadLeftMenu();
    });
});

$(document).on("click", ".QCInspectionProgressReport", function () {
    $.get("/QCInspection/QCInspectionProgressReportIndex", function (data) {
        $('#page-wrapper').html(data);
        LoadLeftMenu();
    });
});

function ViewProjectDocumentList(id) {
    var url = "/QCInspection/ProjectDocumentList?ProjectID=" + id;
    $.get(url, function (data) {
        $('#page-wrapper').html(data);
    });
}

function ViewRFWIProgressReport(id) {
    var url = "/QCInspection/RFWIProgressReport?ID=" + id;
    $.get(url, function (data) {
        $('#page-wrapper').html(data);
    });
}

function ViewQCInspectionProgressReport(id) {
    var url = "/QCInspection/QCInspectionProgressReport?ID=" + id;
    $.get(url, function (data) {
        $('#page-wrapper').html(data);
    });
}

// /////// QCInspection Project Module - Begin////////////////////////////////

$(document).on("click", ".QCInspectionProjectIndex", function () {
    $.get("/QCInspection/ProjectIndex", function (data) {
        $('#page-wrapper').html(data);
    });
});

function CreateQCInspectionProject() {
    $.get("QCInspection/ProjectCreate", function (data) {
        $('#page-wrapper').html(data);
    });
}

function EditQCInspectionProject(id) {
    var url = "QCInspection/ProjectEdit/" + id;
    $.get(url, function (data) {
        $('#page-wrapper').html(data);
    });
}
// /////// QCInspection Project Module - End////////////////////////////////

// /////// Trades Module - Begin////////////////////////////////
$(document).on("click", ".QCInspectionTradeIndex", function () {
    $('#pageloader-overlay').attr('style', 'display:');
    $.get("/QCInspection/TradeIndex", function (data) {
        $('#page-wrapper').html(data);
        LoadLeftMenu();
    });
    $('#pageloader-overlay').attr('style', 'display:none');
});

function CreateTrade() {
    $.get("QCInspection/TradeCreate", function (data) {
        $('#page-wrapper').html(data);
    });
}

function EditTrade(id) {
    var url = "QCInspection/TradeEdit/" + id;
    $.get(url, function (data) {
        $('#page-wrapper').html(data);
    });
}
// /////// Trades Module - End////////////////////////////////


// /////// Defect Types Module - Begin////////////////////////////////
$(document).on("click", ".QCInspectionDefectTypeIndex", function () {
    $('#pageloader-overlay').attr('style', 'display:');
    $.get("/QCInspection/DefectTypeIndex", function (data) {
        $('#page-wrapper').html(data);
        LoadLeftMenu();
    });
    $('#pageloader-overlay').attr('style', 'display:none');
});

function CreateDefectType() {
    $.get("QCInspection/DefectTypeCreate", function (data) {
        $('#page-wrapper').html(data);
    });
}

function EditDefectType(id) {
    var url = "QCInspection/DefectTypeEdit/" + id;
    $.get(url, function (data) {
        $('#page-wrapper').html(data);
    });
}
// /////// Defect Types Module - End////////////////////////////////


// /////// Subcontractor Module - Begin////////////////////////////////
$(document).on("click", ".QCInspectionSubcontractorIndex", function () {
    $('#pageloader-overlay').attr('style', 'display:');
    $.get("/QCInspection/SubcontractorIndex", function (data) {
        $('#page-wrapper').html(data);
        LoadLeftMenu();
    });
    $('#pageloader-overlay').attr('style', 'display:none');
});

function CreateSubcontractor() {
    $.get("QCInspection/SubcontractorCreate", function (data) {
        $('#page-wrapper').html(data);
    });
}

function EditSubcontractor(id) {
    var url = "QCInspection/SubcontractorEdit/" + id;
    $.get(url, function (data) {
        $('#page-wrapper').html(data);
    });
}
// /////// Subcontractor Module - End////////////////////////////////


// /////// RFWI General Check Lists Module - Begin////////////////////////////////
$(document).on("click", ".QCInspectionRFWIGeneralCheckListIndex", function ()
{
    $('#pageloader-overlay').attr('style', 'display:');
    $.get("/QCInspection/RFWIGeneralCheckListIndex", function (data) {
        $('#page-wrapper').html(data);
        LoadLeftMenu();
    });
    $('#pageloader-overlay').attr('style', 'display:none');
});

function CreateRFWIGeneralCheckList()
{
    $.get("QCInspection/RFWIGeneralCheckListCreate", function (data)
    {
        $('#page-wrapper').html(data);
    });
}

function EditRFWIGeneralCheckList(id) {
    var url = "QCInspection/RFWIGeneralCheckListEdit/" + id;
    $.get(url, function (data) {
        $('#page-wrapper').html(data);
    });
}
// /////// RFWIGeneralCheckLists Module - End////////////////////////////////

// /////// RFWI Trades Module - Begin////////////////////////////////
$(document).on("click", ".QCInspectionRFWITradeIndex", function () {
    
    $('#pageloader-overlay').attr('style', 'display:');
    $.get("/QCInspection/RFWITradeIndex", function (data) {
        $('#page-wrapper').html(data);
        LoadLeftMenu();
    });
    $('#pageloader-overlay').attr('style', 'display:none');
});

function CreateRFWITrade() {
    $.get("QCInspection/RFWITradeCreate", function (data) {
        $('#page-wrapper').html(data);
    });
}

function EditRFWITrade(id) {
    var url = "QCInspection/RFWITradeEdit/" + id;
    $.get(url, function (data) {
        $('#page-wrapper').html(data);
    });
}
// /////// RFWI Trades Module - End////////////////////////////////


// /////// QCInspection Defect Form Module - Begin////////////////////////////////
$(document).on("click", ".QCInspectionDefectFormIndex", function () {
    $('#pageloader-overlay').attr('style', 'display:');
    $.get("/QCInspection/QCInspectionIndex", function (data) {
        $('#page-wrapper').html(data);
        LoadLeftMenu();
    });
    $('#pageloader-overlay').attr('style', 'display:none');
});

function CreateQCInspectionDefectForm() {
    $.get("QCInspection/QCInspectionCreate", function (data) {
        $('#page-wrapper').html(data);
    });
}

function EditQCInspectionDefectForm(id) {
    var url = "QCInspection/QCInspectionEdit/" + id;
    $.get(url, function (data) {
        $('#page-wrapper').html(data);
    });
}
// /////// QCInspection Defect Form Module - End////////////////////////////////

/////// RFWI Form Module - Begin////////////////////////////////
$(document).on("click", ".RFWIFormIndex", function () {
    $('#pageloader-overlay').attr('style', 'display:');
    $.get("/QCInspection/RFWIFormIndex", function (data) {
        $('#page-wrapper').html(data);
        LoadLeftMenu();
    });
    $('#pageloader-overlay').attr('style', 'display:none');
});

function CreateRFWIForm() {
    $.get("QCInspection/RFWIFormCreate", function (data) {
        $('#page-wrapper').html(data);
    });
}

function EditRFWIForm(id) {
    var url = "QCInspection/RFWIFormEdit/" + id;
    $.get(url, function (data) {
        $('#page-wrapper').html(data);
    });
}
// /////// RFWI Form Module - End////////////////////////////////


// ****************************************** QC Inspection Module - Begin ***************************************

function removeImageBlanks(imageBase64) {
    debugger
    var imageObject = document.createElement('img');
    imageObject.setAttribute('src', imageBase64);
    imageObject.width = 300;
    imageObject.height = 300;
    imgWidth = imageObject.width;
    imgHeight = imageObject.height;
    var canvas = document.createElement('canvas');
    canvas.setAttribute("width", imgWidth);
    canvas.setAttribute("height", imgHeight);
    var context = canvas.getContext('2d');
    context.drawImage(imageObject, 0, 0);

    var imageData = context.getImageData(0, 0, imgWidth, imgHeight),
        data = imageData.data,
        getRBG = function (x, y) {
            var offset = imgWidth * y + x;
            return {
                red: data[offset * 4],
                green: data[offset * 4 + 1],
                blue: data[offset * 4 + 2],
                opacity: data[offset * 4 + 3]
            };
        },
        isWhite = function (rgb) {
            // many images contain noise, as the white is not a pure #fff white
            //return rgb.red > 200 && rgb.green > 200 && rgb.blue > 200;
            return rgb.red === 255 && rgb.green === 255 && rgb.blue === 255;
        },
        scanY = function (fromTop) {
            var offset = fromTop ? 1 : -1;

            // loop through each row
            for (var y = fromTop ? 0 : imgHeight - 1; fromTop ? (y < imgHeight) : (y > -1); y += offset) {

                // loop through each column
                for (var x = 0; x < imgWidth; x++) {
                    var rgb = getRBG(x, y);
                    if (!isWhite(rgb)) {
                        if (fromTop) {
                            return y;
                        } else {
                            return Math.min(y + 1, imgHeight);
                        }
                    }
                }
            }
            return null; // all image is white
        },
        scanX = function (fromLeft) {
            var offset = fromLeft ? 1 : -1;

            // loop through each column
            for (var x = fromLeft ? 0 : imgWidth - 1; fromLeft ? (x < imgWidth) : (x > -1); x += offset) {
                debugger
                // loop through each row
                for (var y = 0; y < imgHeight; y++) {
                    var rgb = getRBG(x, y);
                    if (!isWhite(rgb))
                    {
                        if (fromLeft)
                        {
                            return x;
                        } else {
                            return Math.min(x + 1, imgWidth);
                        }
                    }
                }
            }
            return null; // all image is white
        };

    var cropTop = scanY(true),
        cropBottom = scanY(false),
        cropLeft = scanX(true),
        cropRight = scanX(false),
        cropWidth = cropRight - cropLeft,
        cropHeight = cropBottom - cropTop;

    canvas.setAttribute("width", cropWidth);
    canvas.setAttribute("height", cropHeight);
    // finally crop the guy
    canvas.getContext("2d").drawImage(imageObject,
        cropLeft, cropTop, cropWidth, cropHeight,
        0, 0, cropWidth, cropHeight);

    return canvas.toDataURL();
}
