# Сборщик частоты слов

## Функционал

Консольное приложение имеет одну основную команду `collect`, которая загружает страницу по переданному адресу и выводит
в консоль частоту встречаемых на ней слов.

После сборки приложение обобщенно запускается командой:

```bash
  dotnet PageStatistics.dll collect $address --threshold=$treshold
```

* collect - название команды.
* $address - ссылка на анализируемую страницу.
* $threshold - порог частоты. Слова, встреченные реже, чем порог частоты, не будут выведены в статистике. Опциональный
  аргумент, по-умолчанию равен 5.

Для получения небольшой справки можно передать параметр -h:

```bash
  dotnet PageStatistics.dll -h
  dotnet PageStatistics.dll collect -h
```

## Сборка и запуск

### SDK

Приложение можно собрать с помощью .NET Core 3.1 SDK. Сборка выполняется командой:

```bash
  dotnet publish PageStatistics -o Release
```

Пример запуска после сборки:

```bash
  cd Release
  dotnet PageStatistics.dll collect "https://docs.microsoft.com/ru-ru/dotnet/core/tools/dotnet-publish"
```

### Docker

Альтернативно сборку можно выполнить с помощью Docker:

```bash
  docker build -t pagestatistics .
```

Запуск при такой сборке:

```bash
  docker run \
    --mount type=bind,source=$PWD/.data,target=/app/.data \
    pagestatistics \
    collect "https://docs.docker.com/storage/volumes/" \
    --threshold=20
```

## Cтек

Приложение написано на C#, с использованием .NET Core и библиотек для отдельных функий (EF Core, HtmlAgilityPack,
Serilog). При этом данные о скачанных страницах и статистика слов хранятся в локальной SQLite базе (которая вместе с
файлом логов и скачанными страницами находится в подпапке `.data` текущей директории).
