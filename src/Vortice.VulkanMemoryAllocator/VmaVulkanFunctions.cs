﻿// Copyright © Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

namespace Vortice.Vulkan;

public unsafe struct VmaVulkanFunctions
{
    public delegate* unmanaged[Stdcall]<VkInstance, sbyte*, delegate* unmanaged[Stdcall]<void>> vkGetInstanceProcAddr;
    public delegate* unmanaged[Stdcall]<VkDevice, sbyte*, delegate* unmanaged[Stdcall]<void>> vkGetDeviceProcAddr;
    public delegate* unmanaged[Stdcall]<VkPhysicalDevice, out VkPhysicalDeviceProperties, void> vkGetPhysicalDeviceProperties;
    public delegate* unmanaged[Stdcall]<VkPhysicalDevice, out VkPhysicalDeviceMemoryProperties, void> vkGetPhysicalDeviceMemoryProperties;
    public delegate* unmanaged[Stdcall]<VkDevice, VkMemoryAllocateInfo*, VkAllocationCallbacks*, out VkDeviceMemory, VkResult> vkAllocateMemory;
    public delegate* unmanaged[Stdcall]<VkDevice, VkDeviceMemory, VkAllocationCallbacks*, void> vkFreeMemory;
    public delegate* unmanaged[Stdcall]<VkDevice, VkDeviceMemory, ulong, ulong, VkMemoryMapFlags, void*, VkResult> vkMapMemory;
    public delegate* unmanaged[Stdcall]<VkDevice, VkDeviceMemory, void> vkUnmapMemory;
    public delegate* unmanaged[Stdcall]<VkDevice, int, VkMappedMemoryRange*, VkResult> vkFlushMappedMemoryRanges;
    public delegate* unmanaged[Stdcall]<VkDevice, int, VkMappedMemoryRange*, VkResult> vkInvalidateMappedMemoryRanges;
    public delegate* unmanaged[Stdcall]<VkDevice, VkBuffer, VkDeviceMemory, ulong, VkResult> vkBindBufferMemory;
    public delegate* unmanaged[Stdcall]<VkDevice, VkImage, VkDeviceMemory, ulong, VkResult> vkBindImageMemory;
    public delegate* unmanaged[Stdcall]<VkDevice, VkBuffer, out VkMemoryRequirements, void> vkGetBufferMemoryRequirements;
    public delegate* unmanaged[Stdcall]<VkDevice, VkImage, out VkMemoryRequirements, void> vkGetImageMemoryRequirements;
    public delegate* unmanaged[Stdcall]<VkDevice, VkBufferCreateInfo*, VkAllocationCallbacks*, out VkBuffer, VkResult> vkCreateBuffer;
    public delegate* unmanaged[Stdcall]<VkDevice, VkBuffer, VkAllocationCallbacks*, void> vkDestroyBuffer;
    public delegate* unmanaged[Stdcall]<VkDevice, VkImageCreateInfo*, VkAllocationCallbacks*, out VkImage, VkResult> vkCreateImage;
    public delegate* unmanaged[Stdcall]<VkDevice, VkImage, VkAllocationCallbacks*, void> vkDestroyImage;
    public delegate* unmanaged[Stdcall]<VkCommandBuffer, VkBuffer, VkBuffer, int, VkBufferCopy*, void> vkCmdCopyBuffer;

    /// Fetch "vkGetBufferMemoryRequirements2" on Vulkan >= 1.1, fetch "vkGetBufferMemoryRequirements2KHR" when using VK_KHR_dedicated_allocation extension.
    public delegate* unmanaged[Stdcall]<VkDevice, VkBufferMemoryRequirementsInfo2*, VkMemoryRequirements2*, void> vkGetBufferMemoryRequirements2KHR;
    /// Fetch "vkGetImageMemoryRequirements2" on Vulkan >= 1.1, fetch "vkGetImageMemoryRequirements2KHR" when using VK_KHR_dedicated_allocation extension.
    public delegate* unmanaged[Stdcall]<VkDevice, VkImageMemoryRequirementsInfo2*, VkMemoryRequirements2*, void> vkGetImageMemoryRequirements2KHR;
    /// Fetch "vkBindBufferMemory2" on Vulkan >= 1.1, fetch "vkBindBufferMemory2KHR" when using VK_KHR_bind_memory2 extension.
    public delegate* unmanaged[Stdcall]<VkDevice, int, VkBindBufferMemoryInfo*, VkResult> vkBindBufferMemory2KHR;
    /// Fetch "vkBindImageMemory2" on Vulkan >= 1.1, fetch "vkBindImageMemory2KHR" when using VK_KHR_bind_memory2 extension.
    public delegate* unmanaged[Stdcall]<VkDevice, int, VkBindImageMemoryInfo*, VkResult> vkBindImageMemory2KHR;
    public delegate* unmanaged[Stdcall]<VkPhysicalDevice, VkPhysicalDeviceMemoryProperties2*, void> vkGetPhysicalDeviceMemoryProperties2KHR;
    /// Fetch from "vkGetDeviceBufferMemoryRequirements" on Vulkan >= 1.3, but you can also fetch it from "vkGetDeviceBufferMemoryRequirementsKHR" if you enabled extension VK_KHR_maintenance4.
    public delegate* unmanaged[Stdcall]<VkDevice, VkDeviceBufferMemoryRequirements*, VkMemoryRequirements2*, void> vkGetDeviceBufferMemoryRequirements;
    /// Fetch from "vkGetDeviceImageMemoryRequirements" on Vulkan >= 1.3, but you can also fetch it from "vkGetDeviceImageMemoryRequirementsKHR" if you enabled extension VK_KHR_maintenance4.
    public delegate* unmanaged[Stdcall]<VkDevice, VkDeviceImageMemoryRequirements*, VkMemoryRequirements2*, void> vkGetDeviceImageMemoryRequirements;  
}
