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

            const newWindow = window.open(linkHref, '_blank');

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
            });
        });
    });
}