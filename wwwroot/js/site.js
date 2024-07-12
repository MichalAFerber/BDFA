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

function attachProfileLinkListeners() {
    document.querySelectorAll('.profile-link').forEach(link => {
        link.addEventListener('click', function (event) {
            event.preventDefault();
            const profileId = this.getAttribute('data-profile-id');
            const linkHref = this.getAttribute('href');
            const clickDateTime = new Date().toISOString();

            fetch('/track-click', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify({
                    profileId: profileId,
                    link: linkHref,
                    clickDateTime: clickDateTime
                })
            }).then(response => {
                if (response.ok) {
                    console.log('Click tracked successfully');
                } else {
                    console.error('Failed to track click');
                    window.location.href = linkHref; // Navigate to the link even if tracking fails
                }
            }).catch(error => {
                console.error('Error:', error);
                window.location.href = linkHref; // Navigate to the link even if tracking fails
            });
        });
    });
}

// Call this function after the PartialView is loaded
attachProfileLinkListeners();