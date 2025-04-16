document.addEventListener('DOMContentLoaded', function () {
    document.querySelectorAll('.quantity-value').forEach(input => {
        input.addEventListener('input', async function () {
            const productId = this.dataset.productId;

            let value = this.value.replace(/[^0-9]/g, '').replace(/^0+/, '') || '1';
            value = Math.max(1, value);

            await updateCartItem(productId, value, this)
            updateTotal();
        });
    });

    document.querySelectorAll('.quantity-control').forEach(button => {
        button.addEventListener('click', async function () {
            const input = this.parentElement.querySelector('.quantity-value');
            const productId = input.dataset.productId;
            let value = parseInt(input.value);

            if (this.classList.contains('plus')) {
                value++;
            }
            else if (this.classList.contains('minus')) {
                value = Math.max(1, value - 1);
            }

            await updateCartItem(productId, value, input);
            updateTotal();
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
                    throw new Error('Ошибка при удалении предмета из корзины');
                }
            }
            catch (error) {
                console.error('Ошибка:', error);
            }
        });
    });

    async function updateCartItem(productId, quantity, inputElement) {
        try {
            const response = await fetch('Cart/UpdateQuantity', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify({
                    productId: parseInt(productId),
                    quantity: quantity
                })
            });

            if (!response.ok) throw new Error('Ошибка обновления');

            inputElement.value = quantity;
        }
        catch (error) {
            console.error('Ошибка:', error);
        }
    }

    async function updateTotal() {
        try {
            const response = await fetch('/Cart/GetTotal');
            if (response.ok) {
                const data = await response.json();
                document.querySelector('.total-amount').textContent = `${data.total} руб.`;
            }
        }
        catch (error) {
            console.error('Ошибка при обновлении суммы:', error);
        }
    }

    updateTotal();
});