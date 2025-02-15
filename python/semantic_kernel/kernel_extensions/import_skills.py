# Copyright (c) Microsoft. All rights reserved.

import glob
import importlib
import inspect
import os
import sys
from typing import Dict

from semantic_kernel.kernel_extensions.extends_kernel import ExtendsKernel
from semantic_kernel.orchestration.sk_function_base import SKFunctionBase
from semantic_kernel.semantic_functions.prompt_template import PromptTemplate
from semantic_kernel.semantic_functions.prompt_template_config import (
    PromptTemplateConfig,
)
from semantic_kernel.semantic_functions.semantic_function_config import (
    SemanticFunctionConfig,
)
from semantic_kernel.utils.validation import validate_skill_name


class ImportSkills(ExtendsKernel):
    def import_native_skill_from_directory(
        self, parent_directory: str, skill_directory_name: str
    ) -> Dict[str, SKFunctionBase]:
        PYTHON_FILE = "native_function.py"

        kernel = self.kernel()

        validate_skill_name(skill_directory_name)

        skill_directory = os.path.abspath(
            os.path.join(parent_directory, skill_directory_name)
        )
        native_py_file_path = os.path.join(skill_directory, PYTHON_FILE)

        if not os.path.exists(native_py_file_path):
            raise ValueError(
                f"Native Skill Python File does not exist: {native_py_file_path}"
            )

        skill_name = os.path.basename(skill_directory)
        try:
            sys.path.append(skill_directory)
            module_name = os.path.splitext(os.path.basename(native_py_file_path))[0]
            module = importlib.import_module(module_name)
        except Exception:
            return {}
        finally:
            sys.path.remove(skill_directory)

        # Find the class in the module
        classes = inspect.getmembers(module, inspect.isclass)
        class_name = next(
            (name for name, cls in classes if inspect.getmodule(cls) == module), None
        )

        if class_name:
            skill_obj = getattr(module, class_name)()
            return kernel.import_skill(skill_obj, skill_name)
        else:
            return {}

    def import_semantic_skill_from_directory(
        self, parent_directory: str, skill_directory_name: str
    ) -> Dict[str, SKFunctionBase]:
        CONFIG_FILE = "config.json"
        PROMPT_FILE = "skprompt.txt"

        kernel = self.kernel()

        validate_skill_name(skill_directory_name)

        skill_directory = os.path.join(parent_directory, skill_directory_name)
        skill_directory = os.path.abspath(skill_directory)

        if not os.path.exists(skill_directory):
            raise ValueError(f"Skill directory does not exist: {skill_directory_name}")

        skill = {}

        directories = glob.glob(skill_directory + "/*/")
        for directory in directories:
            dir_name = os.path.dirname(directory)
            function_name = os.path.basename(dir_name)
            prompt_path = os.path.join(directory, PROMPT_FILE)

            # Continue only if the prompt template exists
            if not os.path.exists(prompt_path):
                continue

            config = PromptTemplateConfig()
            config_path = os.path.join(directory, CONFIG_FILE)
            with open(config_path, "r") as config_file:
                config = config.from_json(config_file.read())

            # Load Prompt Template
            with open(prompt_path, "r") as prompt_file:
                template = PromptTemplate(
                    prompt_file.read(), kernel.prompt_template_engine, config
                )

            # Prepare lambda wrapping AI logic
            function_config = SemanticFunctionConfig(config, template)

            skill[function_name] = kernel.register_semantic_function(
                skill_directory_name, function_name, function_config
            )

        return skill
