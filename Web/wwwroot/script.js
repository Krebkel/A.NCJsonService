$(document).ready(function() {
    loadOrders();
    loadWares();
    setDefaultDateTime();

    // Операции над заказами
    $("#createOrder").click(createOrder);
    $("#ordersTable").on("click", ".editOrder", editOrder);
    $("#ordersTable").on("click", ".deleteOrder", deleteOrder);
    $("#ordersTable").on("click", ".viewPositions", viewPositions);

    // Операции над позициями
    $("#createPosition").click(createPosition);
    $("#positionsTable").on("click", ".deletePosition", deletePosition);

    // Операции над товарами
    $("#createWare").click(createWare);
    $("#waresTable").on("click", ".editWare", editWare);
    $("#waresTable").on("click", ".deleteWare", deleteWare);

    // Операции импорта и экспорта заказов
    $("#importOrdersBtn").click(() => $("#importOrdersFile").click());
    $("#importOrdersFile").change(importOrders);
    $("#exportOrdersBtn").click(exportOrders);

    // Операции импорта и экспорта товаров
    $("#importWaresBtn").click(() => $("#importWaresFile").click());
    $("#importWaresFile").change(importWares);
    $("#exportWaresBtn").click(exportWares);

    // Обработка событий
    $("#loadEvents").click(loadEvents);
    $("#importEvents").click(importEvents);
    $("#postEvent").click(postEvent);
});

// Вывод заказов
function loadOrders() {
    $.ajax({
        url: '/api/orders',
        method: 'GET',
        success: function(orders) {
            $("#ordersTable tbody").empty();
            orders.forEach(function(order) {
                addOrderToTable(order);
            });
        }
    });
}

// Добавление строки с заказом
function addOrderToTable(order) {
    const orderSum = calculateOrderSum(order.id);
    $("#ordersTable tbody").append(`
        <tr data-id="${order.id}">
            <td>${order.id}</td>
            <td>${order.number}</td>
            <td class="orderSum">${orderSum}</td>
            <td>
                <button class="editOrder">Edit</button>
                <button class="deleteOrder">Delete</button>
                <button class="viewPositions">View Positions</button>
            </td>
        </tr>
    `);
}

// Создание заказ
function createOrder() {
    const order = {
        number: prompt("Enter order number:"),
        date: new Date().toISOString(),
        note: prompt("Enter order note:")
    };

    $.ajax({
        url: '/api/orders',
        method: 'POST',
        contentType: 'application/json',
        data: JSON.stringify(order),
        success: function(newOrder) {
            addOrderToTable(newOrder);
        }
    });
}

//  Изменение заказ
function editOrder() {
    const row = $(this).closest('tr');
    const orderId = row.data('id');
    const order = {
        id: orderId,
        number: prompt("Enter new order number:"),
        date: new Date().toISOString(),
        note: prompt("Enter new order note:")
    };

    $.ajax({
        url: `/api/orders/${orderId}`,
        method: 'PUT',
        contentType: 'application/json',
        data: JSON.stringify(order),
        success: function() {
            row.find('td:eq(1)').text(order.number);
            updateOrderSum(orderId);
        }
    });
}

// Удаление заказ
function deleteOrder() {
    const row = $(this).closest('tr');
    const orderId = row.data('id');

    $.ajax({
        url: `/api/orders/${orderId}`,
        method: 'DELETE',
        success: function() {
            row.remove();
        }
    });
}

// Показ позиций заказа
function viewPositions() {
    const orderId = $(this).closest('tr').data('id');
    loadPositions(orderId);
}

// Вывод позиций заказа
function loadPositions(orderId) {
    $.ajax({
        url: `/api/positions/byOrder/${orderId}`,
        method: 'GET',
        success: function(positions) {
            $("#positionsTable tbody").empty();
            positions.forEach(function(position) {
                addPositionToTable(position);
            });
        }
    });
}

// Добавление позиции в таблицу
function addPositionToTable(position) {
    const wareValue = getWareValue(position.wareId);
    const wareName = getWareName(position.wareId);
    const positionSum = position.quantity * wareValue;
    $("#positionsTable tbody").append(`
        <tr data-id="${position.id}">
            <td>${position.id}</td>
            <td>${position.orderId}</td>
            <td>${wareName}</td>
            <td>${position.quantity}</td>
            <td>${positionSum}</td>
            <td>
                <button class="editPosition">Edit</button>
                <button class="deletePosition">Delete</button>
            </td>
        </tr>
    `);
}

// Создание позиции
function createPosition() {
    const position = {
        orderId: prompt("Enter order ID:"),
        wareId: prompt("Enter ware ID:"),
        quantity: prompt("Enter quantity:")
    };

    $.ajax({
        url: `/api/positions/${position.orderId}/positions`,
        method: 'POST',
        contentType: 'application/json',
        data: JSON.stringify(position),
        success: function(newPosition) {
            addPositionToTable(newPosition);
        }
    });
}

// Удаление позиции
function deletePosition() {
    const row = $(this).closest('tr');
    const positionId = row.data('id');
    const orderId = row.find('td:eq(1)').text();

    $.ajax({
        url: `/api/positions/${positionId}`,
        method: 'DELETE',
        success: function() {
            row.remove();
            updateOrderSum(orderId);
        }
    });
}

// Вывод товаров
function loadWares() {
    $.ajax({
        url: '/api/wares',
        method: 'GET',
        success: function(wares) {
            $("#waresTable tbody").empty();
            wares.forEach(function(ware) {
                addWareToTable(ware);
            });
        }
    });
}

// Добавление товара в таблицу
function addWareToTable(ware) {
    $("#waresTable tbody").append(`
        <tr data-id="${ware.id}">
            <td>${ware.id}</td>
            <td>${ware.name}</td>
            <td>${ware.value}</td>
            <td>${ware.property}</td>
            <td>
                <button class="editWare">Edit</button>
                <button class="deleteWare">Delete</button>
            </td>
        </tr>
    `);
}

// Создание товара
function createWare() {
    const ware = {
        name: prompt("Enter ware name:"),
        value: prompt("Enter ware value:"),
        property: prompt("Enter ware property:")
    };

    $.ajax({
        url: '/api/wares',
        method: 'POST',
        contentType: 'application/json',
        data: JSON.stringify(ware),
        success: function(newWare) {
            addWareToTable(newWare);
        }
    });
}

// Изменение товара
function editWare() {
    const row = $(this).closest('tr');
    const wareId = row.data('id');
    const ware = {
        id: wareId,
        name: prompt("Enter new ware name:"),
        value: prompt("Enter new ware value:"),
        property: prompt("Enter new ware property:")
    };

    $.ajax({
        url: `/api/wares/${wareId}`,
        method: 'PUT',
        contentType: 'application/json',
        data: JSON.stringify(ware),
        success: function() {
            row.find('td:eq(1)').text(ware.name);
            row.find('td:eq(2)').text(ware.value);
            row.find('td:eq(3)').text(ware.property);
        }
    });
}

// Удаление товара
function deleteWare() {
    const row = $(this).closest('tr');
    const wareId = row.data('id');

    $.ajax({
        url: `/api/wares/${wareId}`,
        method: 'DELETE',
        success: function() {
            row.remove();
        }
    });
}

// Импорт заказов из JSON
function importOrders() {
    const fileInput = document.getElementById('importOrdersFile');
    const file = fileInput.files[0];

    const formData = new FormData();
    formData.append('importFile', file);

    $.ajax({
        url: '/api/orders/import',
        method: 'POST',
        data: formData,
        contentType: false,
        processData: false,
        success: function() {
            alert('Orders imported successfully');
            loadOrders();
        }
    });
}

// Экспорт товаров в JSON
function exportOrders() {
    window.location.href = '/api/orders/export';
}

// Импорт товаров из JSON
function importWares() {
    const fileInput = document.getElementById('importWaresFile');
    const file = fileInput.files[0];

    const formData = new FormData();
    formData.append('importFile', file);

    $.ajax({
        url: '/api/wares/import',
        method: 'POST',
        data: formData,
        contentType: false,
        processData: false,
        success: function() {
            alert('Wares imported successfully');
            loadWares();
        }
    });
}

// Экспорт товаров в JSON
function exportWares() {
    window.location.href = '/api/wares/export';
}

// Импорт событий из JSON
function importEvents() {
    const fileInput = document.getElementById('importEventsFile');
    const file = fileInput.files[0];
    formData.append('importFile', file);

    $.ajax({
        url: '/api/events/import',
        method: 'POST',
        data: formData,
        contentType: false,
        processData: false,
        success: function() {
            alert('Events imported successfully');
            loadEvents();
        }
    });
}

// Загрузка в БД события
function postEvent() {
    const event = {
        name: $("#eventName").val(),
        value: $("#eventValue").val()
    };

    $.ajax({
        url: '/api/events',
        method: 'POST',
        contentType: 'application/json',
        data: JSON.stringify(event),
        success: function() {
            alert("Event posted successfully");
            loadEvents();
        }
    });
}

// Время по дефолту +-30 минут
function setDefaultDateTime() {
    const now = new Date();
    const startTime = new Date(now.getTime() - 30 * 60000);
    const endTime = new Date(now.getTime() + 30 * 60000);

    const formatDateTime = (date) => {
        return date.toISOString().slice(0, 16);
    };

    $("#startDate").val(formatDateTime(startTime));
    $("#endDate").val(formatDateTime(endTime));
}

// Поиск событий по периоду
function loadEvents() {
    const startTime = formatDateTime($("#startDate").val(), 'DD.MM.YYYY hh:mm:ss');
    const endTime = formatDateTime($("#endDate").val(), 'DD.MM.YYYY hh:mm:ss');

    $.ajax({
        url: '/api/events',
        method: 'GET',
        data: {
            startTime: startTime,
            endTime: endTime
        },
        success: function(events) {
            $("#eventsTable tbody").empty();
            $.each(events, function(time, value) {
                $("#eventsTable tbody").append(`
                    <tr>
                        <td>${formatDateTime(time, 'DD.MM.YYYY hh:mm')}</td>
                        <td>${value}</td>
                    </tr>
                `);
            });
        }
    });
}

// Расчет суммы позиций заказа
function calculateOrderSum(orderId) {
    let sum = 0;
    $.ajax({
        url: `/api/positions/byOrder/${orderId}`,
        method: 'GET',
        async: false,
        success: function(positions) {
            positions.forEach(function(position) {
                sum += position.quantity * getWareValue(position.wareId);
            });
        }
    });
    return sum;
}

// Поиск цены товара по ID
function getWareValue(wareId) {
    let value = 0;
    $.ajax({
        url: `/api/wares/${wareId}`,
        method: 'GET',
        async: false,
        success: function(ware) {
            value = ware.value;
        }
    });
    return value;
}

// Поиск наименования товара по ID
function getWareName(wareId) {
    let value = 0;
    $.ajax({
        url: `/api/wares/${wareId}`,
        method: 'GET',
        async: false,
        success: function(ware) {
            name = ware.name;
        }
    });
    return name;
}

// Обновление суммы в заказе
function updateOrderSum(orderId) {
    const sum = calculateOrderSum(orderId);
    $(`#ordersTable tr[data-id="${orderId}"] .orderSum`).text(sum);
}

// Форматирование времени
function formatDateTime(dateTime, format) {
    const date = new Date(dateTime);
    const map = {
        'DD': String(date.getDate()).padStart(2, '0'),
        'MM': String(date.getMonth() + 1).padStart(2, '0'),
        'YYYY': date.getFullYear(),
        'hh': String(date.getHours()).padStart(2, '0'),
        'mm': String(date.getMinutes()).padStart(2, '0'),
        'ss': String(date.getSeconds()).padStart(2, '0')
    };

    return format.replace(/DD|MM|YYYY|hh|mm|ss/g, matched => map[matched]);
}