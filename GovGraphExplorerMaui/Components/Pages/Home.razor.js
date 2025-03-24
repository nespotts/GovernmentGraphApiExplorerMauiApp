

export function initializeInputListener() {
    let textarea = document.getElementById('endpoint-textarea');
    textarea.addEventListener('keydown', (event) => {
        console.log('keydown', event);
        if (event.key === 'Enter') {
            event.preventDefault();
            //document.getElementById('endpoint-button').click();
        }
    });
}