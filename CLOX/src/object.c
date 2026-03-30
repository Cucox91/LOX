#include <stdio.h>
#include <string.h>

#include "memory.h"
#include "object.h"
#include "value.h"
#include "vm.h"

#define ALLOCATE_OBJ(type, objectType) \
    (type *)allocateObject(sizeof(type), objectType)

static ObjString *allocateString(char *chars, int lenght)
{
    ObjString *string = ALLOCATE_OBJ(ObjString, OBJ_STRING);
    string->length = lenght;
    string->chars = chars;
    return string;
}

ObjString *copyString(const char *chars, int length)
{
    char *heapChar = ALLOCATE(char, length + 1);
    memcpy(heapChar, chars, length);
    heapChar[length] = '\0';
    return allocateString(heapChar, length);
}
