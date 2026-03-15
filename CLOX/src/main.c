#include "common.h"
#include "chunk.h"
#include "debug.h"

int main(int argc, const char *argv[])
{
    Chunk chunk;
    initChunk(&chunk);

    // This is a two-byte instruction. Added one by one.
    int constant = addConstant(&chunk, 1.2); // Add and get the index.
    writeChunk(&chunk, OP_CONSTANT, 123);    // Say is a Constant.
    writeChunk(&chunk, constant, 123);       // Save the index.

    writeChunk(&chunk, OP_RETURN, 123);

    disassembleChunk(&chunk, "Test Chunk");
    freeChunk(&chunk);

    return 0;
}
