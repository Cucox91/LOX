#ifndef clox_chunk_h
#define clox_chunk_h

#include "common.h"

// Here we define the one-byte Operation Codes.
typedef enum
{
    OP_RETURN, // Return from Current Function.
} OpCode;

// This struct will be a dynamic array of chunks of code.
typedef struct
{
    int count;
    int capacity;
    uint8_t *code;
} Chunk;

void initChunk(Chunk *chunk);

void writeChunk(Chunk *chunk, uint8_t byte);
void freeChunk(Chunk *chunk);

#endif