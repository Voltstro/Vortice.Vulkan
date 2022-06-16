// Copyright © Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

using System.Numerics;
using Vortice.Vulkan;
using static Vortice.Vulkan.Vulkan;
using static Vortice.Vulkan.VMA;

namespace DrawTriangle;

public static unsafe class Program
{
#if DEBUG
    private static bool EnableValidationLayers = true;
#else
	private static bool EnableValidationLayers = false;
#endif

    public static void Main()
    {
        using TestApp testApp = new TestApp();
        testApp.Run();
    }

    class TestApp : Application
    {
        private GraphicsDevice _graphicsDevice;
        public override string Name => "02-DrawTriangle";

        private VkPipelineLayout _pipelineLayout;
        private VkPipeline _pipeline;
        private VkBuffer _vertexBuffer;
        private VmaAllocation _vertexBufferMemory;
        private VmaAllocator _allocator;

        protected override void Initialize()
        {
            _graphicsDevice = new GraphicsDevice(Name, EnableValidationLayers, MainWindow);

            VmaAllocatorCreateInfo allocatorInfo = new()
            {
                //flags = VmaAllocatorCreateFlags.KHRDedicatedAllocation | VmaAllocatorCreateFlags.KHRBindMemory2,
                physicalDevice = _graphicsDevice.PhysicalDevice,
                device = _graphicsDevice.VkDevice,
                instance = _graphicsDevice.VkInstance,
                vulkanApiVersion = VkVersion.Version_1_2
            };
            VkResult result = vmaCreateAllocator(&allocatorInfo, out _allocator);

            VkPipelineLayoutCreateInfo pipelineLayoutCreateInfo = new()
            {
                sType = VkStructureType.PipelineLayoutCreateInfo
            };
            vkCreatePipelineLayout(_graphicsDevice, &pipelineLayoutCreateInfo, null, out _pipelineLayout).CheckResult();

            // Create pipeline
            {
                VkString entryPoint = new("main");

                CreateShaderModule("triangle.vert", out VkShaderModule vertexShader);
                CreateShaderModule("triangle.frag", out VkShaderModule fragmentShader);

                VkPipelineShaderStageCreateInfo* shaderStages = stackalloc VkPipelineShaderStageCreateInfo[2];
                // Vertex shader
                shaderStages[0].sType = VkStructureType.PipelineShaderStageCreateInfo;
                shaderStages[0].stage = VkShaderStageFlags.Vertex;
                shaderStages[0].module = vertexShader;
                shaderStages[0].pName = entryPoint;

                // Fragment shader
                shaderStages[1].sType = VkStructureType.PipelineShaderStageCreateInfo;
                shaderStages[1].stage = VkShaderStageFlags.Fragment;
                shaderStages[1].module = fragmentShader;
                shaderStages[1].pName = entryPoint;

                // VertexInputState
                VkVertexInputBindingDescription vertexInputBinding = default;
                vertexInputBinding.binding = 0;
                vertexInputBinding.stride = (uint)VertexPositionColor.SizeInBytes;
                vertexInputBinding.inputRate = VkVertexInputRate.Vertex;

                VkVertexInputAttributeDescription* vertexInputAttributs = stackalloc VkVertexInputAttributeDescription[2];
                // Attribute location 0: Position
                vertexInputAttributs[0].binding = 0;
                vertexInputAttributs[0].location = 0;
                vertexInputAttributs[0].format = VkFormat.R32G32B32SFloat;
                vertexInputAttributs[0].offset = 0;
                // Attribute location 1: Color
                vertexInputAttributs[1].binding = 0;
                vertexInputAttributs[1].location = 1;
                vertexInputAttributs[1].format = VkFormat.R32G32B32A32SFloat;
                vertexInputAttributs[1].offset = 12;

                VkPipelineVertexInputStateCreateInfo vertexInputState = new();
                vertexInputState.sType = VkStructureType.PipelineVertexInputStateCreateInfo;
                vertexInputState.vertexBindingDescriptionCount = 1;
                vertexInputState.pVertexBindingDescriptions = &vertexInputBinding;
                vertexInputState.vertexAttributeDescriptionCount = 2;
                vertexInputState.pVertexAttributeDescriptions = vertexInputAttributs;

                VkPipelineInputAssemblyStateCreateInfo inputAssemblyState = VkPipelineInputAssemblyStateCreateInfo.New();
                inputAssemblyState.topology = VkPrimitiveTopology.TriangleList;

                VkPipelineViewportStateCreateInfo viewportState = new()
                {
                    sType = VkStructureType.PipelineViewportStateCreateInfo,
                    viewportCount = 1,
                    scissorCount = 1
                };

                // Rasterization state
                VkPipelineRasterizationStateCreateInfo rasterizationState = new()
                {
                    sType = VkStructureType.PipelineRasterizationStateCreateInfo,
                    polygonMode = VkPolygonMode.Fill,
                    cullMode = VkCullModeFlags.None,
                    frontFace = VkFrontFace.CounterClockwise,
                    depthClampEnable = false,
                    rasterizerDiscardEnable = false,
                    depthBiasEnable = false,
                    lineWidth = 1.0f
                };

                // Multi sampling state
                VkPipelineMultisampleStateCreateInfo multisampleState = new();
                multisampleState.sType = VkStructureType.PipelineMultisampleStateCreateInfo;
                multisampleState.rasterizationSamples = VkSampleCountFlags.Count1;
                multisampleState.pSampleMask = null;

                // DepthStencil
                VkPipelineDepthStencilStateCreateInfo depthStencilState = new();
                depthStencilState.sType = VkStructureType.PipelineDepthStencilStateCreateInfo;
                depthStencilState.depthTestEnable = true;
                depthStencilState.depthWriteEnable = true;
                depthStencilState.depthCompareOp = VkCompareOp.LessOrEqual;
                depthStencilState.depthBoundsTestEnable = false;
                depthStencilState.back.failOp = VkStencilOp.Keep;
                depthStencilState.back.passOp = VkStencilOp.Keep;
                depthStencilState.back.compareOp = VkCompareOp.Always;
                depthStencilState.stencilTestEnable = false;
                depthStencilState.front = depthStencilState.back;

                // BlendStates
                VkPipelineColorBlendAttachmentState blendAttachmentState = default;
                blendAttachmentState.colorWriteMask = VkColorComponentFlags.All;
                blendAttachmentState.blendEnable = false;

                VkPipelineColorBlendStateCreateInfo colorBlendState = new();
                colorBlendState.sType = VkStructureType.PipelineColorBlendStateCreateInfo;
                colorBlendState.attachmentCount = 1;
                colorBlendState.pAttachments = &blendAttachmentState;

                // Dynamic states
                VkDynamicState* dynamicStateEnables = stackalloc VkDynamicState[2];
                dynamicStateEnables[0] = VkDynamicState.Viewport;
                dynamicStateEnables[1] = VkDynamicState.Scissor;

                VkPipelineDynamicStateCreateInfo dynamicState = new()
                {
                    sType = VkStructureType.PipelineDynamicStateCreateInfo,
                    dynamicStateCount = 2,
                    pDynamicStates = dynamicStateEnables
                };

                VkGraphicsPipelineCreateInfo pipelineCreateInfo = new()
                {
                    sType = VkStructureType.GraphicsPipelineCreateInfo,
                    stageCount = 2,
                    pStages = shaderStages,
                    pVertexInputState = &vertexInputState,
                    pInputAssemblyState = &inputAssemblyState,
                    pTessellationState = null,
                    pViewportState = &viewportState,
                    pRasterizationState = &rasterizationState,
                    pMultisampleState = &multisampleState,
                    pDepthStencilState = &depthStencilState,
                    pColorBlendState = &colorBlendState,
                    pDynamicState = &dynamicState,
                    layout = _pipelineLayout,
                    renderPass = _graphicsDevice.Swapchain.RenderPass
                };

                // Create rendering pipeline using the specified states
                vkCreateGraphicsPipeline(_graphicsDevice, pipelineCreateInfo, out _pipeline).CheckResult();

                vkDestroyShaderModule(_graphicsDevice, vertexShader);
                vkDestroyShaderModule(_graphicsDevice, fragmentShader);
            }

            {
                // Create vertex buffer
                ReadOnlySpan<VertexPositionColor> sourceData = new VertexPositionColor[]
                {
                    new VertexPositionColor(new Vector3(0f, 0.5f, 0.0f), new Vector4(1.0f, 0.0f, 0.0f, 1.0f)),
                    new VertexPositionColor(new Vector3(0.5f, -0.5f, 0.0f), new Vector4(0.0f, 1.0f, 0.0f, 1.0f)),
                    new VertexPositionColor(new Vector3(-0.5f, -0.5f, 0.0f), new Vector4(0.0f, 0.0f, 1.0f, 1.0f))
                };

                uint vertexBufferSize = (uint)(sourceData.Length * VertexPositionColor.SizeInBytes);

                VkBufferCreateInfo vertexBufferInfo = new()
                {
                    sType = VkStructureType.BufferCreateInfo,
                    size = vertexBufferSize,
                    // Buffer is used as the copy source
                    usage = VkBufferUsageFlags.TransferSrc
                };

                // Create a host-visible buffer to copy the vertex data to (staging buffer)
                VmaAllocationCreateInfo memoryInfo = new()
                {
                    flags = VmaAllocationCreateFlags.HostAccessSequentialWrite | VmaAllocationCreateFlags.Mapped,
                    usage = VmaMemoryUsage.Auto
                };

                VmaAllocationInfo allocationInfo;
                vmaCreateBuffer(_allocator, &vertexBufferInfo,
                    &memoryInfo,
                    out VkBuffer stagingBuffer,
                    out VmaAllocation stagingBufferAllocation,
                    &allocationInfo).CheckResult();

                // Map and copy
                void* pMappedData = allocationInfo.pMappedData;
                Span<VertexPositionColor> destinationData = new(pMappedData, sourceData.Length);
                sourceData.CopyTo(destinationData);

                vertexBufferInfo.usage = VkBufferUsageFlags.VertexBuffer | VkBufferUsageFlags.TransferDst;
                vmaCreateBuffer(_allocator, &vertexBufferInfo, out _vertexBuffer, out _vertexBufferMemory).CheckResult();

                VkCommandBuffer copyCmd = _graphicsDevice.GetCommandBuffer(true);

                // Put buffer region copies into command buffer
                VkBufferCopy copyRegion = default;

                // Vertex buffer
                copyRegion.size = vertexBufferSize;
                vkCmdCopyBuffer(copyCmd, stagingBuffer, _vertexBuffer, 1, &copyRegion);

                // Flushing the command buffer will also submit it to the queue and uses a fence to ensure that all commands have been executed before returning
                _graphicsDevice.FlushCommandBuffer(copyCmd);

                vmaDestroyBuffer(_allocator, stagingBuffer, stagingBufferAllocation);
            }
        }


        public override void Dispose()
        {
            _graphicsDevice.WaitIdle();

            vkDestroyPipelineLayout(_graphicsDevice, _pipelineLayout);
            vkDestroyPipeline(_graphicsDevice, _pipeline);
            vmaDestroyBuffer(_allocator, _vertexBuffer, _vertexBufferMemory);
            vmaDestroyAllocator(_allocator);

            _graphicsDevice.Dispose();

            base.Dispose();
        }

        protected override void OnTick()
        {
            _graphicsDevice!.RenderFrame(OnDraw);
        }

        private void OnDraw(VkCommandBuffer commandBuffer, VkFramebuffer framebuffer, VkExtent2D size)
        {
            VkClearValue clearValue = new VkClearValue(0.0f, 0.0f, 0.2f, 1.0f);

            // Begin the render pass.
            VkRenderPassBeginInfo renderPassBeginInfo = new()
            {
                sType = VkStructureType.RenderPassBeginInfo,
                renderPass = _graphicsDevice.Swapchain.RenderPass,
                framebuffer = framebuffer,
                renderArea = new VkRect2D(size),
                clearValueCount = 1,
                pClearValues = &clearValue
            };

            vkCmdBeginRenderPass(commandBuffer, &renderPassBeginInfo, VkSubpassContents.Inline);

            // Update dynamic viewport state
            // Flip coordinate to map DirectX coordinate system.
            VkViewport viewport = new()
            {
                x = 0.0f,
                y = MainWindow.Extent.height,
                width = MainWindow.Extent.width,
                height = -MainWindow.Extent.height,
                minDepth = 0.0f,
                maxDepth = 1.0f
            };
            vkCmdSetViewport(commandBuffer, viewport);

            // Update dynamic scissor state
            VkRect2D scissor = new(MainWindow.Extent);
            vkCmdSetScissor(commandBuffer, scissor);

            // Bind the rendering pipeline
            vkCmdBindPipeline(commandBuffer, VkPipelineBindPoint.Graphics, _pipeline);

            // Bind triangle vertex buffer (contains position and colors)
            vkCmdBindVertexBuffer(commandBuffer, 0, _vertexBuffer);

            // Draw non indexed
            vkCmdDraw(commandBuffer, 3, 1, 0, 0);

            vkCmdEndRenderPass(commandBuffer);
        }

        private void CreateShaderModule(string name, out VkShaderModule shaderModule)
        {
            byte[] vertexBytecode = File.ReadAllBytes(Path.Combine(AppContext.BaseDirectory, "Assets", $"{name}.spv"));
            _graphicsDevice.CreateShaderModule(vertexBytecode, out shaderModule).CheckResult();
        }
    }
}
