#include <stdio.h>

#include "common.h"
#include "compiler.h"
#include "scanner.h"

void compile(const char *source)
{
    initScanner(source);
    int line = -1;
    for (;;)
    {
        Token token = scanToken();
        if (token.line != line)
        {
            printf("%4d ", token.line);
            line = token.line;
        }
        else
        {
            printf("    |");
        }

        // This is another way to display values. Kind of a substring.
        // This is enabled by the %.*s and passing the lenght and the start.
        printf("%2d '%.*s'\n", token.type, token.lenght, token.start);
        if (token.type == TOKEN_EOF)
        {
            break;
        }
    }
}
