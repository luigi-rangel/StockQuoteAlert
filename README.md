# StockQuoteAlert

CLI application for Brazilian stock market quote monitoring. Past an upper or lower threshold, the app sends an email notification.

The application uses [brapi](brapi.dev) to retrieve the quotes. The api is free for up to 15,000 requests a month. It is necessary to have an account and generate an api token.

## Settings

The quote api, as well as the email information, can be configured in the configuration file `App.config`. An [example file](https://github.com/luigi-rangel/StockQuoteAlert/blob/master/App.config.example) is provided.

Apart from the quote api token, the configuration file also needs to be filled with mailing information, such as the SMTP server connection fields, the sender, and the receiver.

## Usage

After building the .NET application, open the command terminal in the program folder and execute:

`stock-quote-alert.exe <asset> <low> <high>`

- **asset** is the B3 code of the monitored asset
- **low** is the lower notification threshold
- **high** is the upper notification threshold

## Architectural and Project choices

The code is simple and exports one single functionality. It focuses on modularity where needed -- the main program file abstracts the quote api and the mail service, which work independently.

After some research on quote apis, *brapi* was the best I found: it offers up to 15k requests a month for free, with 5 minutes of delay to the actual stock market. I could not find any suitable real-time, publisher/subscriber apis, nor an api with a lower delay. However, when testing the api, I noticed the api releases quote updates once a minute for the **preceding minute**. This led to the solution presented by the program: a main loop, inside which the quote api is called **every minute**.

## Generative AI declaration

The bulk of the coding research was done through official documentation websites and stackoverflow. Code completion offered by Visual Studio was used for convenience.
