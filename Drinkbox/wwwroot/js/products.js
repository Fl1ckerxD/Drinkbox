document.addEventListener('DOMContentLoaded', function () {
    const brandSelect = document.querySelector('.brand-select');
    const priceRange = document.querySelector('input[type="range"]');

    /**
     * Обновляет минимальную и максимальную цены в зависимости от выбранного бренда.
     * @param {number|null} brandId - ID выбранного бренда (null, если бренд не выбран).
     */
    async function updatePriceValues(brandId) {
        try {
            const maxPriceValueLabel = document.querySelector('.max-price');
            const minPriceValueLabel = document.querySelector('.min-price');

            const response = await fetch(`/Home/GetPriceValues?brandId=${brandId}`);
            if (!response.ok) throw new Error('Ошибка сети');

            const data = await response.json();
            const maxPrice = Math.ceil(data.maxPrice);
            const minPrice = Math.ceil(data.minPrice);

            // Обновляем диапазон и значение ползунка
            priceRange.min = minPrice;
            priceRange.max = maxPrice;
            priceRange.value = maxPrice;

            // Обновляем текстовые метки с ценами
            minPriceValueLabel.textContent = `${minPrice} руб.`
            maxPriceValueLabel.textContent = `${maxPrice} руб.`;
        }
        catch (error) {
            console.error('Ошибка:', error);
            maxPriceValueLabel.textContent = "0";
            minPriceValueLabel.textContent = "0";
        }
    }

    /**
     * Обновляет состояние кнопки корзины в зависимости от статуса корзины.
     */
    async function updateCartButton() {
        const response = await fetch('/Cart/GetCartStatus');
        const data = await response.json();

        const cartButton = document.querySelector('.choose-button');
        const text = document.querySelector('.cart');

        // Если в корзине есть товары, обновляем ссылку и текст
        if (data.hasItems) {
            cartButton.href = '/Cart';
            text.textContent = `Выбрано: ${data.itemCount}`;
        }
        else {
            cartButton.removeAttribute('href');
            text.textContent = 'Не выбрано';
        }
    }

    // Инициализация: обновляем цены при загрузке страницы
    updatePriceValues(null);

    /**
     * Обрабатывает изменение выбора бренда.
     */
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

            // Обновляем диапазон цен для выбранного бренда
            await updatePriceValues(brandId);
        }
        catch (error) {
            console.error('Ошибка:', error);
            document.querySelector('.product-grid').innerHTML = `
                            <div class="error">Ошибка загрузки данных</div>
                        `;
        }
    });

    /**
     * Обрабатывает изменение значения ползунка цены.
     */
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

    /**
     * Обрабатывает клик по кнопке выбора продукта.
     */
    document.addEventListener('click', async function (event) {
        const button = event.target.closest('.product-button');
        if (!button) return;

        const productId = button.dataset.productId;
        const isSelected = button.dataset.selected === 'true';

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

            toggleButtonState(button);
            updateCartButton();
        }
        catch (error) {
            console.error('Ошибка:', error);
        }
    });

    /**
     * Переключает состояние кнопки выбора продукта.
     * @param {HTMLElement} button - Кнопка выбора продукта.
     */
    function toggleButtonState(button) {
        if (!button) return;

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

    document.getElementById('excelFileInput').addEventListener('change', function (e) {
        const fileName = this.files[0]?.name || 'Файл не выбран';

        // Автоматическая отправка формы при выборе файла
        if (this.files.length > 0) {
            this.closest('form').submit();
        }
    });
});