# HRM System - Система управления персоналом

## Описание проекта

Система управления персоналом (HRM) - это WPF-приложение для автоматизации процессов управления сотрудниками компании. Система охватывает ведение личных дел, учет рабочего времени, управление отпусками и больничными, расчет заработной платы и другие HR-процессы.

## Основные возможности

- **Управление сотрудниками**:
  - Добавление, редактирование, удаление сотрудников
  - Просмотр полной информации о сотрудниках
  - Импорт/экспорт данных в JSON

- **Управление отделами**:
  - Создание и редактирование отделов
  - Назначение менеджеров отделов

- **Управление должностями**:
  - Создание должностей
  - Указание руководящих должностей

## Технологии

- **Frontend**: WPF, XAML, MVVM
- **Backend**: .NET 6
- **Database**: PostgreSQL
- **ORM**: Dapper
- **DI**: Microsoft.Extensions.DependencyInjection

## Установка и запуск

1. **Требования**:
   - .NET 6 SDK
   - PostgreSQL 12+
   - Visual Studio 2022 (рекомендуется)

2. **Настройка базы данных**:
   - Создайте базу данных в PostgreSQL
   - Выполните SQL-скрипты из папки `DatabaseScripts`
   - Настройте строку подключения в `appsettings.json`

3. **Запуск приложения**:
   ```bash
   git clone https://github.com/Lalka5008/hrmApp.git
   cd hrmApp
   dotnet restore
   dotnet run
   ```

## Архитектура

### Слоистая архитектура (Layered Architecture)

1. **Presentation Layer**:
   - XAML-представления (`MainWindow.xaml`, `EmployeeView.xaml`)
   - Окна редактирования

2. **Application Layer**:
   - ViewModels (`EmployeeViewModel.cs`, `DepartmentViewModel.cs`)
   - Команды и бизнес-логика

3. **Domain Layer**:
   - Модели данных (`Employee`, `Department`, `Position`)
   - Интерфейсы сервисов

4. **Data Access Layer**:
   - Сервисы (`EmployeeService.cs`, `DepartmentService.cs`)
   - Репозитории (Dapper)

### Принципы SOLID

- **Single Responsibility**: Каждый класс отвечает за одну задачу
- **Open-Closed**: Возможность расширения без изменения существующего кода
- **Liskov Substitution**: Возможность замены реализаций через интерфейсы
- **Interface Segregation**: Минимальные интерфейсы сервисов
- **Dependency Inversion**: Зависимости через абстракции

## JSON-формат для импорта/экспорта

Пример структуры JSON-файла для сотрудников:

```json
[
  {
    "EmployeeId": 1,
    "FirstName": "Иван",
    "LastName": "Иванов",
    "BirthDate": "1985-05-15T00:00:00",
    "Gender": "Male",
    "DepartmentId": 1,
    "PositionId": 1,
    "HireDate": "2020-01-10T00:00:00",
    "Status": "Active",
    "DepartmentName": "IT",
    "PositionTitle": "Developer"
  }
]
```

## Скриншоты

![Главное окно](https://github.com/user-attachments/assets/30d4d92d-410b-44fa-b711-1588074a8097)
![Таблица с сотрудниками](https://github.com/user-attachments/assets/b9489343-14a6-4ca4-90f7-85c8d4ea3397)



## Лицензия

MIT License
