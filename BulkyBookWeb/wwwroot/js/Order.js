var dataTable;

$(document).ready(function () {
    var url = window.location.search;
    var status = "all";
    if (url.includes("inProcess")) {
        status = "inProcess"
    }
    else if (url.includes("pending"))
    {
        status = "pending"
    }
    else if(url.includes("completed"))
    {
        status = "pending"
    }
    loadDataTable(status);
});

function loadDataTable(status) {
    dataTable = $('#tblData').DataTable({
        "ajax": {
            "url":`/Admin/Order/GetAll?status=${status}`
        },
        "columns": [
            {"data":"id", "width":"5%"},
            {"data":"user.name", "width":"25%"},
            {"data":"user.phoneNumber", "width":"15%"},
            {"data":"user.email", "width":"15%"},
            {"data":"orderStatus", "width":"15%"},
            {"data":"orderTotal", "width":"15%"},
            {
                "data": "id",
                "render": function (data) {
                    return `
                        <div class="w-75 btn-group" role="group">
							<a href="/Admin/Order/Detail?id=${data}" class="btn btn-primary mx-2"> <i class="bi bi-pencil-square"></i> Details</a>
						</div>`
                },
                "width":"10%"
            }

        ]
    });
}