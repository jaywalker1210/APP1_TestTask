# Inventory System

Инвентарная система для игры с сохранением, drag-and-drop и экономикой.

## Функционал системы

### Экономика
- Баланс монет (добавление/трата)
- Сохранение монет между сессиями

### Инвентарь
- 50 слотов (5 в ряду)
- 15 слотов открыто изначально
- Пошаговая разблокировка слотов за монеты
- Настраиваемая стоимость каждого слота
- Drag-and-drop предметов
- Объединение стаков (если предмет стакается)
- Обмен предметов местами
- Подсчёт общего веса инвентаря
- Всплывающие подсказки с описанием предметов

### Сохранение
- Состояние разблокировки слотов
- Расположение предметов
- Количество предметов в стаках

### Gameplay
- Добавление случайного оружия/брони
- Добавление патронов (с умным стаканием)
- Стрельба из случайного оружия
- Удаление случайного предмета

## Архитектура проекта
Assets/_Project/Scripts/
Core/
EconomyService.cs # Управление монетами
GameManager.cs # Главный контроллер
SaveService.cs # Сохранение/загрузка
ItemDatabase.cs # База предметов

Inventory/
InventoryStorage.cs # Хранилище (модель)
InventorySlotData.cs # Данные слота
InventoryItem.cs # Данные предмета
InventorySlotUnlocker.cs# Логика разблокировки

Services/
CombatService.cs # Механика стрельбы
LootService.cs # Генерация лута
WeightCalculator.cs # Расчёт веса

UI/
MainUIManager.cs # Главное UI
InventorySlot.cs # UI слота (drag&drop)
Components/
CoinsUI.cs
TooltipUI.cs

Data/
ItemData.cs # Базовый класс предмета
WeaponData.cs # Оружие
ArmorData.cs # Броня
AmmoData.cs # Патроны

Saving/
ISaveSystem.cs # Интерфейс сохранения
JsonSaveSystem.cs # JSON реализация

## Технологии
- Unity 2022.3.62f
- C#
- JSON (сохранение)
- Unity UI + TextMeshPro

## Установка

1. Клонировать репозиторий
2. Открыть проект в Unity
3. В сцене настроить ссылки в инспекторе:
   - GameManager → MainUIManager, InventoryStorage, ItemDatabase
   - MainUIManager → все UI элементы
4. Запустить сцену

## Примеры использования

// Добавление монет
economy.AddCoins(100);

// Разблокировка слота
unlocker.TryUnlockSlot(5, settings);

// Стрельба
combat.TryShoot(storage, out int damage, out string log);

// Добавление предмета
loot.TryAddRandomItem(storage, weapons, armors, out string log);

## Сохраняемые файлы
coins.json - баланс монет (C:\Users\User\AppData\LocalLow\название_компании(в Unity)\название_вашего_проекта(в Unity))

inventory.json - состояние инвентаря (C:\Users\User\AppData\LocalLow\название_компании(в Unity)\название_вашего_проекта(в Unity))

## Автор
Салихов Жасур
jasok5108@gmail.com