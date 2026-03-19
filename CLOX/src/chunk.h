#ifndef clox_chunk_h
#define clox_chunk_h

#include "common.h"
#include "value.h"

// Here we define the one-byte Operation Codes.
typedef enum
{
    OP_CONSTANT, // Produce a Constant. (2B)
    OP_RETURN,   // Return from Current Function. (1B)
    OP_NEGATE,   // Negate a Value (1B)

    // Arithmetic Operators (1B yes!)
    OP_ADD,
    OP_SUBTRACT,
    OP_MULTIPLY,
    OP_DIVIDE,
} OpCode;

// This struct will be a dynamic array of chunks of code.
typedef struct
{
    int count;
    int capacity;
    uint8_t *code;
    int *lines;
    ValueArray constants;
} Chunk;

void initChunk(Chunk *chunk);
void writeChunk(Chunk *chunk, uint8_t byte, int line);
void freeChunk(Chunk *chunk);

int addConstant(Chunk *chunk, Value value);

#endif