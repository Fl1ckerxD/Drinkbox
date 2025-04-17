document.addEventListener('DOMContentLoaded', function () {

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

    function updatePaymentTotal() {
        let total = 0;

        document.querySelectorAll('.sum-cell').forEach(cell => {
            total += parseInt(cell.textContent.split()[0]) || 0;
        });

        document.querySelector('.payment-total-amount').textContent = `${total} руб.`;
        return total;
    }

    function getCoinsData() {
        const coins = [];

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

    async function checkPayment() {
        const cartTotal = await updateTotal();
        const paymentTotal = updatePaymentTotal();
        const paymentButton = document.querySelector('.payment-button');

        if (paymentTotal >= cartTotal) {
            paymentButton.classList.remove('disabled');
        }
        else {
            paymentButton.classList.add('disabled');
        }
    }

    async function updateSumPrice(element) {
        const row = element.closest('tr');
        const denomination = parseInt(row.querySelector('.coin-name').textContent.split()[0]);
        const quantity = parseInt(element.value) || 0;
        const sumCell = row.querySelector('.sum-cell');

        sumCell.textContent = `${denomination * quantity} руб.`;
    }

    updateTotal();

    document.querySelectorAll('.quantity-value').forEach(input => {
        input.addEventListener('input', async function () {
            //const coinId = this.dataset.coinId;

            let value = this.value.replace(/[^0-9]/g, '').replace(/^0+/, '') || '0';
            this.value = Math.max(0, value);

            updateSumPrice(this);
            updatePaymentTotal();
            checkPayment();
        });
    });

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
            updateSumPrice(input);
            updatePaymentTotal();
            checkPayment();
        });
    });

    document.querySelector('.payment-button').addEventListener('click', async function (e) {
        e.preventDefault();

        if (this.classList.contains('disabled')) return;

        const coins = getCoinsData();

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
            }
        }
        catch (error) {
            console.error('Ошибка:', error);
            alert('Произошла ошибка при обработке платежа');
        }
    })

    updateTotal();
    updatePaymentTotal();
    checkPayment();
});