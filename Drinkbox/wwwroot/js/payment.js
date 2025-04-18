document.addEventListener('DOMContentLoaded', function () {

    /**
     * Обновляет общую сумму товаров в корзине через API.
     * @returns {number} - Возвращает общую стоимость товаров в корзине.
     */
    async function updateTotal() {
        try {
            const response = await fetch('/Cart/GetTotal');
            if (response.ok) {
                const data = await response.json();
                document.querySelector('.total-amount').textContent = `${data.total} руб.`;
                return data.total;
            }
        }
        catch (error) {
            console.error('Ошибка при обновлении суммы:', error);
        }
    }

    /**
     * Обновляет общую сумму платежа на основе данных из таблицы монет.
     * @returns {number} - Возвращает общую сумму платежа.
     */
    function updatePaymentTotal() {
        let total = 0;

        document.querySelectorAll('.sum-cell').forEach(cell => {
            total += parseInt(cell.textContent.split()[0]) || 0;
        });

        document.querySelector('.payment-total-amount').textContent = `${total} руб.`;
        return total;
    }

    /**
     * Получает данные о монетах из таблицы.
     * @returns {Array} - Массив объектов с данными о монетах.
     */
    function getCoinsData() {
        const coins = [];

        // Проходим по всем строкам таблицы монет
        document.querySelectorAll('.payment-table tbody tr').forEach(row => {
            const quantityInput = row.querySelector('.quantity-value');
            const coinName = row.querySelector('.coin-name');

            if (quantityInput && coinName) {
                coins.push({
                    CoinId: parseInt(quantityInput.dataset.coinId),
                    Value: parseInt(coinName.textContent.split()[0]),
                    Quantity: parseInt(quantityInput.value) || 0
                });
            }
        });

        return coins.filter(coin => coin.Quantity > 0);
    }

    /**
     * Проверяет, достаточно ли средств для оплаты товаров в корзине.
     */
    async function checkPayment() {
        const cartTotal = await updateTotal();
        const paymentTotal = updatePaymentTotal();
        const paymentButton = document.querySelector('.payment-button');
        const sumPrice = document.querySelector('.payment-total-amount');

        if (paymentTotal >= cartTotal) {
            paymentButton.classList.remove('disabled');
            sumPrice.style.color = '#7BC043';
        }
        else {
            paymentButton.classList.add('disabled');
            sumPrice.style.color = 'red';
        }
    }

    /**
     * Обновляет сумму для строки с монетой в таблице платежей.
     * @param {HTMLElement} element - Элемент поля ввода количества монет.
     */
    async function updateSumPrice(element) {
        const row = element.closest('tr');
        const denomination = parseInt(row.querySelector('.coin-name').textContent.split()[0]);
        const quantity = parseInt(element.value) || 0;
        const sumCell = row.querySelector('.sum-cell');

        sumCell.textContent = `${denomination * quantity} руб.`;
    }

    // Обрабатываем ввод количества монет
    document.querySelectorAll('.quantity-value').forEach(input => {
        input.addEventListener('input', async function () {
            // Очищаем значение от нечисловых символов и удаляем начальные нули
            let value = this.value.replace(/[^0-9]/g, '').replace(/^0+/, '') || '0';
            this.value = Math.max(0, value);

            // Обновляем сумму для строки, общую сумму платежа и проверяем возможность оплаты
            updateSumPrice(this);
            updatePaymentTotal();
            checkPayment();
        });
    });

    // Обрабатываем кнопки "+" и "-" для изменения количества монет
    document.querySelectorAll('.quantity-control').forEach(button => {
        button.addEventListener('click', async function () {
            const input = this.parentElement.querySelector('.quantity-value');
            let value = parseInt(input.value);

            if (this.classList.contains('plus')) {
                value++;
            }
            else if (this.classList.contains('minus')) {
                value = Math.max(0, value - 1);
            }

            input.value = value;

            // Обновляем сумму для строки, общую сумму платежа и проверяем возможность оплаты
            updateSumPrice(input);
            updatePaymentTotal();
            checkPayment();
        });
    });

    // Обрабатываем клик по кнопке оплаты
    document.querySelector('.payment-button').addEventListener('click', async function (e) {
        e.preventDefault(); // Предотвращаем стандартное поведение кнопки

        if (this.classList.contains('disabled')) return;

        const coins = getCoinsData(); // Получаем данные о монетах

        try {
            const response = await fetch('/Payment/ProcessPayment', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify(coins)
            });

            const result = await response.json();

            if (!result.success) {
                alert(result.message);
                return;
            }

            // Перенаправляем пользователя на страницу завершения покупки
            window.location.href = result.redirectUrl;
        }
        catch (error) {
            console.error('Ошибка:', error);
            alert('Произошла ошибка при обработке платежа');
        }
    })

    // Инициализация: обновляем общую сумму, сумму платежа и проверяем возможность оплаты
    updateTotal();
    updatePaymentTotal();
    checkPayment();
});