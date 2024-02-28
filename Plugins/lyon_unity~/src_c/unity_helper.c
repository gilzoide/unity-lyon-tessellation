#include <stdlib.h>

#include "IUnityInterface.h"
#include "IUnityLog.h"

static IUnityLog *logger;

void lyon_unity_log_error(const char *cstr) {
    if (logger) {
        UNITY_LOG_ERROR(logger, cstr);
    }
}

void c_UnityPluginLoad(IUnityInterfaces *unityInterfaces) {
    logger = UNITY_GET_INTERFACE(unityInterfaces, IUnityLog);
}