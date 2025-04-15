document.addEventListener('DOMContentLoaded', function () {
    const brandSelect = document.querySelector('.brand-select');
    const priceRange = document.querySelector('input[type="range"]');

    // Функция для обновления максимальной цены
    async function updatePriceValues(brandId) {
        try {
            const maxPriceValueLabel = document.querySelector('.max-price');
            const minPriceValueLabel = document.querySelector('.min-price');
            const response = await fetch(`/Home/GetPriceValues?brandId=${brandId}`);
            if (!response.ok) throw new Error('Ошибка сети');

            const data = await response.json();
            const maxPrice = Math.ceil(data.maxPrice);
            const minPrice = Math.ceil(data.minPrice);

            //Обновляем диапазон и значение
            priceRange.min = minPrice;
            priceRange.max = maxPrice;
            priceRange.value = maxPrice;
            minPriceValueLabel.textContent = `${minPrice} руб.`
            maxPriceValueLabel.textContent = `${maxPrice} руб.`;
        }
        catch (error) {
            console.error('Ошибка:', error);
            maxPriceValueLabel.textContent = "0";
            minPriceValueLabel.textContent = "0";
        }
    }

    async function updateCartButton() {
        const response = await fetch('/Cart/GetCartStatus');
        const data = await response.json();

        const cartButton = document.querySelector('.choose-button');
        const text = document.querySelector('.cart');

        if (data.hasItems) {
            cartButton.href = '/Cart/GetCart';
            text.textContent = `Выбрано: ${data.itemCount}`;
        }
        else {
            cartButton.removeAttribute('href');
            text.textContent = 'Не выбрано';
        }
    }

    // Инициализация при загрузке
    updatePriceValues(null);

    // Обработчик изменения выбора бренда
    brandSelect.addEventListener('change', async function () {
        try {
            const brandId = this.value;
            const response = await fetch(`/Home/FilterProducts?brandId=${brandId}`, {
                headers: {
                    'Accept': 'text/html'
                }
            });

            if (!response.ok) throw new Error('Ошибка сети');

            const html = await response.text();
            document.querySelector('.product-grid').innerHTML = html;
            updatePriceValues(brandId);
        }
        catch (error) {
            console.error('Ошибка:', error);
            document.querySelector('.product-grid').innerHTML = `
                            <div class="error">Ошибка загрузки данных</div>
                        `;
        }
    });

    priceRange.addEventListener('input', async function () {
        try {
            const brandId = brandSelect.value;
            const priceValue = this.value;
            const response = await fetch(`/Home/FilterProducts?brandId=${brandId}&maxPrice=${priceValue}`, {
                headers: {
                    'Accept': 'text/html'
                }
            });

            if (!response.ok) throw new Error('Ошибка сети');

            const html = await response.text();
            document.querySelector('.product-grid').innerHTML = html;
        }
        catch (error) {
            console.error('Ошибка:', error);
            document.querySelector('.product-grid').innerHTML = `
                            <div class="error">Ошибка загрузки данных</div>
                        `;
        }
    });

    document.querySelectorAll('.product-button').forEach(button => {
        button.addEventListener('click', async function () {
            const productId = this.dataset.productId;
            const isSelected = this.dataset.selected === 'true';

            try {
                const response = await fetch('/Cart/ToggleProduct', {
                    method: 'POST',
                    headers: {
                        'Content-Type': 'application/json'
                    },
                    body: JSON.stringify({
                        productId: parseInt(productId),
                        isSelected: !isSelected
                    })
                });

                if (!response.ok) throw new Error('Ошибка сети');

                toggleButtonState(this);
                updateCartButton();
                alert('Товар был добавлен');
            }
            catch (error) {
                console.error('Ошибка:', error);
            }
        });
    });

    function toggleButtonState(button) {
        const isSelected = button.dataset.selected === 'true';

        button.textContent = isSelected ? 'Выбрать' : 'Выбрано';
        button.dataset.selected = (!isSelected).toString();

        if (isSelected) {
            button.classList.remove('selected');
        }
        else {
            button.classList.add('selected');
        }
    }
});