document.addEventListener('DOMContentLoaded', function () {
    document.querySelectorAll('.quantity-value').forEach(input => {
        input.addEventListener('input', function () {
            this.value = this.value.replace(/[^0-9]/g, '');
        })
    });
})