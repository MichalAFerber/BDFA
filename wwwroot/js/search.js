document.querySelectorAll('.search-input').forEach(input => {
    input.addEventListener('input', function () {
        var searchQuery = input.value;

        fetch(`/Index?handler=ProfilesPartial&searchQuery=${encodeURIComponent(searchQuery)}`)
            .then(response => response.text())
            .then(html => {
                document.getElementById('profilesContainer').innerHTML = html;
            })
            .catch(error => console.error('Error:', error));
    });
});

document.addEventListener("DOMContentLoaded", function () {
    var searchInput = document.getElementById("textSearchBody");

    // Event listener for pressing the Enter key
    searchInput.addEventListener("keypress", function (event) {
        if (event.key === "Enter") {
            event.preventDefault();         }
    });
});