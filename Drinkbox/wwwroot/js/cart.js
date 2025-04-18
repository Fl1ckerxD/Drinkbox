document.addEventListener('DOMContentLoaded', function () {
    // Обрабатываем ввод количества товаров в корзине
    document.querySelectorAll('.quantity-value').forEach(input => {
        input.addEventListener('input', async function () {
            const productId = this.dataset.productId;

            // Очищаем значение от нечисловых символов и удаляем начальные нули. Если значение пустое, устанавливаем 1.
            let value = this.value.replace(/[^0-9]/g, '').replace(/^0+/, '') || '1';
            value = Math.max(1, value);

            // Обновляем количество товара в корзине и обновляем общую сумму
            await updateCartItem(productId, value, this)
            updateTotal();
        });
    });

    // Обрабатываем кнопки "+" и "-" для изменения количества товаров
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

            // Обновляем количество товара в корзине и обновляем общую сумму
            await updateCartItem(productId, value, input);
            updateTotal();
        });
    });

    // Обрабатываем удаление товаров из корзины
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
                    updateTotal();
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

    /**
     * Обновляет количество товара в корзине через API.
     * @param {number} productId - ID товара.
     * @param {number} quantity - количество товара.
     * @param {HTMLElement} inputElement - Элемент поля ввода для обновления значения.
     */
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

            const result = await response.json();

            inputElement.value = result.actualQuantity;
            updateProductPrice(inputElement);
        }
        catch (error) {
            console.error('Ошибка:', error);
        }
    }

    /**
     * Обновляет общую сумму товаров в корзине.
     */
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

    /**
     * Обновляет цену товара в зависимости от его количества.
     * @param {HTMLElement} element - Элемент поля ввода количества.
     */
    async function updateProductPrice(element) {
        const row = element.closest('tr');
        const priceElement = row.querySelector('.price-cell');
        const unitPrice = parseInt(priceElement.dataset.unitPrice);
        const quantity = parseInt(element.value);
        const currentPrice = unitPrice * quantity;
        priceElement.textContent = `${currentPrice} руб.`;
    }

    // Инициализация: обновляем общую сумму при загрузке страницы
    updateTotal();
});