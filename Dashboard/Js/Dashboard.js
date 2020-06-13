var dashboard = {
    constants: {
        instanceRecordTemplate: '<tr><td><input class="expand" type="button" value="+" data-subdomain="{{SubDomain}}" data-id="{{Departmentid}}" /></td><td>{{DepartmentName}}</td><td>{{ElectronicFileCreated}}</td><td>{{ElectronicFilePending}}</td><td>{{ElectronicFileClosed}}</td><td>{{PhysicalFileCreated}}</td><td>{{PhysicalFilePending}}</td><td>{{PhysicalFileClosed}}</td><td>{{ElectronicReceiptCreated}}</td><td>{{PhysicalReceiptCreated}}</td></tr>',
        departmentRecordTemplate: '<tr style="background-color: cadetblue;"><td></td><td>{{DepartmentName}}</td><td>{{UserCount}}</td><td>{{ElectronicFileCreated}}</td><td>{{ElectronicFilePending}}</td><td>{{ElectronicFileClosed}}</td><td>{{ElectronicReceiptCreated}}</td></tr>'
    },
    getDashboardData: function () {
        var table = $('#dashboardTable').DataTable({
            'ajax': 'https://localhost:44362/Home/GetData',
            'columns': [
                {
                    'className': 'details-control',
                    'orderable': false,
                    'data': null,
                    'defaultContent': ''
                },
                { 'data': 'InstanceName' },
                { 'data': 'UserCount' },
                { 'data': 'ElectronicFileCreated' },
                { 'data': 'ElectronicFilePending' },
                { 'data': 'ElectronicFileClosed' },
                { 'data': 'ElectronicReceiptCreated' }
            ],
            'order': [[1, 'asc']],
            'paging': false,
            "footerCallback": function (row, data, start, end, display) {
                var api = this.api();

                console.log(data);
                var totalElectronicFileCreated = 0;
                var totalElectronicFilePending = 0;
                var totalElectronicFileClosed = 0;
                var totalElectronicReceiptCreated = 0;
                var totalActiveUsers = 0;
                $.each(data, function (i, x) {
                    totalElectronicFileCreated = totalElectronicFileCreated + x.ElectronicFileCreated;
                    totalElectronicFilePending = totalElectronicFilePending + x.ElectronicFilePending;
                    totalElectronicFileClosed = totalElectronicFileClosed + x.ElectronicFileClosed;
                    totalElectronicReceiptCreated = totalElectronicReceiptCreated + x.ElectronicReceiptCreated;
                    totalActiveUsers = totalActiveUsers + x.UserCount;
                })

                $('.filescreated').text(totalElectronicFileCreated);
                $('.filesactive').text(totalElectronicFilePending);
                $('.filesclosed').text(totalElectronicFileClosed);
                $('.receiptscreated').text(totalElectronicReceiptCreated);
                $('.userCount').text(totalActiveUsers);

                $(api.column(1).footer()).html(
                    'Total'
                );
                $(api.column(2).footer()).html(
                    totalActiveUsers
                );
                $(api.column(3).footer()).html(
                    totalElectronicFileCreated
                );
                $(api.column(4).footer()).html(
                    totalElectronicFilePending
                );
                $(api.column(5).footer()).html(
                    totalElectronicFileClosed
                );
                $(api.column(6).footer()).html(
                    totalElectronicReceiptCreated
                );
            }
        });

        $('#dashboardTable tbody').on('click', 'td.details-control', function () {

            var tr = $(this).closest('tr');
            var row = table.row(tr);

            if (row.child.isShown()) {
                // This row is already open - close it
                row.child.hide();
                tr.removeClass('shown');
            } else {
                // Open this row
                $.ajax({
                    type: "GET",
                    url: "https://localhost:44362/Home/GetInstanceData?type=Department&subdomain=" + row.data().SubDomain,
                    success: function (response) {
                        var tableRows = '';
                        $.each(response, function (i, x) {
                            x.display = "block";
                            var compiledTemplate = Handlebars.compile(dashboard.constants.departmentRecordTemplate);
                            var html = compiledTemplate(x);
                            tableRows = tableRows + html;
                        });

                        var html = '<div>' +
                            '<table width = "100%" id = "dashboardTableDept" class="display" > ' +
                            '<thead>' +
                                '<tr style="font-weight:700;font-size:large">' +
                                    '<td></td>' +
                                    '<td></td>' +
                                    '<td>Active Users</td>' +
                                    '<td>eFiles Created</td>' +
                                    '<td>eFiles (Active)	</td>'+
                                    '<td>eFiles (Closed)</td>' +
                                    '<td>eReceipt Created</td>' +
                                '</tr>' +
                            '</thead>' +
                            '<tbody id = "dashboardBody" > ' + tableRows +
                            '</tbody > ' +
                        '</table></div>';
                        row.child(html).show();
                        tr.addClass('shown');
                    }
                });
            }
        });
    },
    getInstanceData: function (instanceId, subdomain) {
        $.ajax({
            type: "GET",
            url: "https://localhost:44362/Home/GetInstanceData?type=Department&subdomain=" + subdomain,
            success: function (response) {

                //$.each(response, function (i, x) {
                //    x.display = "block";
                //    var compiledTemplate = Handlebars.compile(dashboard.constants.departmentRecordTemplate);
                //    var html = compiledTemplate(x);
                //    debugger;
                //    $('input.expand[data-subdomain="' + subdomain + '"]').parent().parent().after(html);
                //});
                //$('input.expand[data-subdomain="' + subdomain + '"]').val('-');
                //$('input.expand[data-subdomain="' + subdomain + '"]').data('loaded', 'true');
            }
        });
    },
    getSectionData: function (instanceId, subdomain) {
        $.ajax({
            type: "GET",
            url: "https://localhost:44362/Home/GetInstanceData?type=Department&subdomain=" + subdomain,
            success: function (response) {
                $.each(response, function (i, x) {
                    x.display = "block";
                    var compiledTemplate = Handlebars.compile(dashboard.constants.departmentRecordTemplate);
                    var html = compiledTemplate(x);
                    debugger;
                    $('input.expand[data-subdomain="' + subdomain + '"]').parent().parent().after(html);
                });
                $('input.expand[data-subdomain="' + subdomain + '"]').val('-');
                $('input.expand[data-subdomain="' + subdomain + '"]').data('loaded', 'true');
            }
        });
    }
}