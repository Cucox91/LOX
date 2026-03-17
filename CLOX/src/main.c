#include "common.h"
#include "chunk.h"
#include "debug.h"
#include "vm.h"

int main(int argc, const char *argv[])
{
    initVM();

    Chunk chunk;
    initChunk(&chunk);

    // This is a two-byte instruction. Added one by one.
    int constant = addConstant(&chunk, 1.2); // Add and get the index.
    writeChunk(&chunk, OP_CONSTANT, 123);    // Say is a Constant.
    writeChunk(&chunk, constant, 123);       // Save the index.

    // Large Arithmetic Operation

    constant = addConstant(&chunk, 3.4);
    writeChunk(&chunk, OP_CONSTANT, 123);
    writeChunk(&chunk, constant, 123);

    writeChunk(&chunk, OP_ADD, 123);

    constant = addConstant(&chunk, 5.6);
    writeChunk(&chunk, OP_CONSTANT, 123);
    writeChunk(&chunk, constant, 123);

    writeChunk(&chunk, OP_DIVIDE, 123);

    writeChunk(&chunk, OP_NEGATE, 123);
    writeChunk(&chunk, OP_RETURN, 123);

    disassembleChunk(&chunk, "Test Chunk");
    interpret(&chunk);
    freeVM();
    freeChunk(&chunk);

    return 0;
}
