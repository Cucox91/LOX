#include <stdio.h>
#include <stdlib.h>
#include <string.h>

#include "common.h"
#include "vm.h"

static void repl()
{
    char line[1024];
    for (;;)
    {
        printf("> ");

        if (!fgets(line, sizeof(line), stdin))
        {
            printf("\n");
            break;
        }

        interpret(line);
    }
}

static char *readFile(const char *path)
{
    // Open the file.
    FILE *file = fopen(path, "rb");

    if (file == NULL)
    {
        fprintf(stderr, "Could not open file \"%s\".\n", path);
        exit(74);
    }

    // The next three line read the file and give us the size of it
    // then proceed to reset the cursor to the begin of the file.
    fseek(file, 0L, SEEK_END);
    size_t fileSize = ftell(file);
    rewind(file);

    // We create a buffer with the size of the file plus 1 to store the
    // EOF/NULL BYTE at the end of the buffer.
    // Then we proceed to read the file and attach the
    // EOF/NULL BYTE at the end of the buffer.

    char *buffer = (char *)malloc(fileSize + 1);

    // This verify if there is enough memory available for the file.
    if (buffer == NULL)
    {
        fprintf(stderr, "Not enough memory to read \"%s\".\n", path);
    }

    size_t bytesRead = fread(buffer, sizeof(char), fileSize, file);

    // If fails when reading the file.
    if (bytesRead < fileSize)
    {
        fprintf(stderr, "Could not read file \"%s\".\n", path);
    }

    buffer[bytesRead] = '\0';

    // We close the file and release the memory. And return the buffer data.
    fclose(file);
    return buffer;
}

static void runFile(const char *path)
{
    char *source = readFile(path);
    InterpretResult result = interpret(source);
    free(source);

    if (result == INTERPRET_COMPILE_ERROR)
        exit(65);
    if (result == INTERPRET_RUNTIME_ERROR)
        exit(70);
}

int main(int argc, const char *argv[])
{
    initVM();

    if (argc == 1)
    {
        repl();
    }
    else if (argc == 2)
    {
        runFile(argv[1]);
    }
    else
    {
        fprintf(stderr, "Usage: clox [path]\n");
        exit(64);
    }

    freeVM();
    return 0;
}
