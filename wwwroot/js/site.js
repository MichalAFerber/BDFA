function attachProfileLinkListeners() {
    document.querySelectorAll('.profile-link').forEach(link => {
        link.addEventListener('click', function (event) {
            event.preventDefault();
            const profileId = parseInt(this.getAttribute('data-profile-id'), 10);
            if (isNaN(profileId)) {
                console.error('Invalid ProfileId');
                return;
            }
            const linkHref = this.getAttribute('href');
            const clickDateTime = new Date().toISOString();

            fetch('/track-click', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify({
                    ProfileId: profileId,
                    Link: linkHref,
                    ClickDateTime: clickDateTime
                })
            }).then(response => {
                if (response.ok) {
                    console.log('Click tracked successfully');
                    window.open(linkHref, '_blank');
                } else {
                    console.error('Failed to track click');
                    window.open(linkHref, '_blank');
                }
            }).catch(error => {
                console.error('Error:', error);
                window.open(linkHref, '_blank');
            });
        });
    });
}
