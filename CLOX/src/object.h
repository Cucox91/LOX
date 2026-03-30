#ifndef clox_object_h
#define clox_object_h

#include "common.h"
#include "value.h"

#define OBJ_TYPE(value) (AS_OBJ(value)->type)
#define IS_STRING(value) isObjType(value, OBJ_STRING) // Notice this is a Value.
                                                      // Not an Obj*
#define AS_STRING(value) ((ObjString *)AS_OBJ(value))
#define AS_CSTRING(value) (((ObjString *)AS_OBJ(value))->chars)

typedef enum
{
    OBJ_STRING
} ObjType;

struct Obj
{
    ObjType type;
};

/*
    Some notes to remember on structs:
    Remember that order matters in struct elems declaration for
    memory arrangement. This is why obj goes first here.
    This behavior is usefull for pointer operations in C.
    This "kind of" enable inheritance in C.

    Here is the specific part of the specs: § 6.7.2.1 13
    Within a structure object, the non-bit-field members and the units in which
    bit-fields reside have addresses that increase in the order in which they
    are declared. A pointer to a structure object, suitably converted,
    points to its initial member
    (or if that member is a bit-field, then to the unit in which it resides),
    and vice versa.
    There may be unnamed padding within a structure object,
    but not at its beginning.
*/
struct ObjString
{
    Obj obj;
    int length;
    char *chars;
};

/*
    Notice that this was declare d as a function instead of inside of the macro
    because we are calling 'value' twice inside the function. And this will
    cause the macro to be called twice.
    Raziel: Pleace spend some time reviewing macros in C. It looks like I have a
    gap on my knowledge here.
*/
static inline bool isObjType(Value value, ObjType type)
{
    return IS_OBJ(value) && AS_OBJ(value)->type == type;
}

ObjString* copyString(const char* chars, int lenght);

#endif
