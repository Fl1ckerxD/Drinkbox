document.addEventListener('DOMContentLoaded', function () {
    document.querySelectorAll('.quantity-value').forEach(input => {
        input.addEventListener('input', function () {
            this.value = this.value.replace(/[^0-9]/g, '');
        });
    });

    document.querySelectorAll('.remove-icon').forEach(button => {
        button.addEventListener('click', async function () {
            if (!confirm('Вы уверены, что хотите удалить этот предмет?')) {
                return;
            }

            try {
                const productId = this.dataset.productId;
                const response = await fetch(`/Cart/RemoveFromCart?productId=${productId}`, {
                    method: 'POST'
                });

                if (response.ok) {
                    const listItem = this.closest('tr');
                    listItem.remove();

                    if (document.querySelectorAll('tbody tr').length === 0) {
                        document.querySelector('tbody').innerHTML = '<p>У вас нет ни одного товара, вернитесь на страница каталога</p>'
                    }
                }
                else {
                    alert('Ошибка при удалении предмета из корзины');
                }
            }
            catch (error) {
                console.error('Ошибка:', error);
            }
        });
    });
});