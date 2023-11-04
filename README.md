# ConsoleWebScraper
Free Web Scraper

This project is a simple console-based web scraper built in C#. It allows you to input a URL and it will scrape the HTML content of that URL. It can extract specific elements from the HTML content, such as URLs and images, and save them to separate files.

## Features

- **URL Input**: The program prompts the user to enter a URL. It then sends a GET request to that URL and retrieves the HTML content.

- **HTML Parsing**: The program uses regular expressions to parse the HTML content. It can extract inner URLs and images from the HTML.

- **File Saving**: The program can save the scraped URLs, images, and the HTML content (with HTML tags removed) to separate files.

## Usage

Run the program and when prompted, enter a valid URL. The program will then scrape the HTML content of that URL, extract the inner URLs and images, and save them to separate files.

## Error Handling

The program includes basic error handling. If an exception occurs during the scraping process (for example, if the URL is invalid), the program will catch the exception and print an error message.

## Note

This is a basic web scraper and may not work with all websites, especially those that heavily rely on JavaScript for rendering content or have measures in place to prevent scraping.
