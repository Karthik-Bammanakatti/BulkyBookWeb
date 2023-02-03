﻿var dataTable;

$(document).ready(
    function () {
        loadDataTable();
    }
);

function loadDataTable() {
    dataTable = $("#tblData").DataTable(
        {
            "ajax": {
                "url" : "/Admin/Product/GetAll"
            },
            "columns": [
                { "data": "title", "width": "15%" },
                { "data": "isbn", "width": "15%" },
                { "data": "price", "width": "15%" },
                { "data": "author", "width": "15%" },
                { "data": "category.name", "width": "15%" },
                {
                    "data": "id",
                    "render": function (data) {
                        return `
                            <div class="btn-group w-75" role="group">
                                <div>
                                    <a href="/Admin/Product/Upsert?id=${data}" class="btn mx-3 btn btn-outline-primary text-nowrap"><i class="bi bi-pencil-square"></i>Edit</a>
                                </div>
                                <div>
                                    <a class="btn mx-3 btn btn-outline-danger text-nowrap" onClick = Delete('/Admin/Product/Delete/${data}')><i class="bi bi-trash3"></i>Delete</a>
                                </div>
                            </div>
                        `
                    }, 
                    "width": "15%"
                }
            ]
        }
    )
}

function Delete(url) {
    Swal.fire({
        title: 'Are you sure?',
        text: "You won't be able to revert this!",
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#d33',
        confirmButtonText: 'Yes, delete it!'
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                url: url,
                type: "DELETE",
                success: function (data) {
                    if (data.success) {
                        dataTable.ajax.reload();
                        toastr.success(data.message);
                    } else {
                        toastr.error(data.message);
                    }
                }
            })
        }
    })
}