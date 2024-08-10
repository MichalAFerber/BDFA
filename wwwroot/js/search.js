document.addEventListener("DOMContentLoaded", function () {
    var searchStart = document.getElementById("SearchStart");
    var txtSearch = document.getElementById("searchQuery");

    if (searchStart && searchQuery) {
        searchStart.addEventListener("click", function (event) {
            event.preventDefault(); // Prevent the default anchor behavior
            txtSearch.focus(); // Set focus to the textSearchBody element
        });
    } else {
        console.error("SearchStart or searchQuery element not found");
    }

    function performSearch() {
        var searchQuery = $('#searchQuery').val();
        {
            $.ajax({
                url: '/Index?handler=ProfilesPartial',
                type: 'GET',
                data: { searchQuery: searchQuery },
                success: function (result) {
                    $('#profilesList').html(result);
                },
                error: function (xhr, status, error) {
                    console.error("Error: " + error);
                }
            });
        }
    }

    $(document).ready(function () {
        $('#searchButton').click(function () {
            performSearch();
        });

        $('#searchQuery').on('input', function () {
            performSearch();
        });

        $('#searchQuery').keypress(function (event) {
            if (event.which === 13) { // 13 is the Enter key code
                event.preventDefault(); // Prevent the default form submission
                $('#searchButton').click(); // Trigger the search button click event
            }
        });
    });
});
