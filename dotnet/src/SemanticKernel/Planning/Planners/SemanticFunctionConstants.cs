﻿// Copyright (c) Microsoft. All rights reserved.

namespace Microsoft.SemanticKernel.Planning.Planners;

internal static class SemanticFunctionConstants
{
    internal const string FunctionFlowFunctionDefinition =
        @"Create an XML plan step by step, to satisfy the goal given.
To create a plan, follow these steps:
0. The plan should be as short as possible.
1. From a <goal> create a <plan> as a series of <functions>.
2. Before using any function in a plan, check that it is present in the most recent [AVAILABLE FUNCTIONS] list. If it is not, do not use it. Do not assume that any function that was previously defined or used in another plan or in [EXAMPLES] is automatically available or compatible with the current plan.
3. Only use functions that are required for the given goal.
4. A function has an 'input' and an 'output'.
5. The 'output' from each function is automatically passed as 'input' to the subsequent <function>.
6. 'input' does not need to be specified if it consumes the 'output' of the previous function.
7. To save an 'output' from a <function>, to pass into a future <function>, use <function.{FunctionName} ... setContextVariable: ""<UNIQUE_VARIABLE_KEY>""/>
8. To save an 'output' from a <function>, to return as part of a plan result, use <function.{FunctionName} ... appendToResult: ""RESULT__<UNIQUE_RESULT_KEY>""/>
9. Append an ""END"" XML comment at the end of the plan.

[EXAMPLES]
[AVAILABLE FUNCTIONS]

  _GLOBAL_FUNCTIONS_.BucketOutputs:
    description: When the output of a function is too big, parse the output into a number of buckets.
    inputs:
    - input: The output from a function that needs to be parse into buckets.
    - bucketCount: The number of buckets.
    - bucketLabelPrefix: The target label prefix for the resulting buckets. Result will have index appended e.g. bucketLabelPrefix='Result' => Result_1, Result_2, Result_3

  EmailConnector.LookupContactEmail:
    description: looks up the a contact and retrieves their email address
    inputs:
    - input: the name to look up

  EmailConnector.EmailTo:
    description: email the input text to a recipient
    inputs:
    - input: the text to email
    - recipient: the recipient's email address. Multiple addresses may be included if separated by ';'.

  LanguageHelpers.TranslateTo:
    description: translate the input to another language
    inputs:
    - input: the text to translate
    - translate_to_language: the language to translate to

  WriterSkill.Summarize:
    description: summarize input text
    inputs:
    - input: the text to summarize

[END AVAILABLE FUNCTIONS]

<goal>Summarize the input, then translate to japanese and email it to Martin</goal>
<plan>
  <function.WriterSkill.Summarize/>
  <function.LanguageHelpers.TranslateTo translate_to_language=""Japanese"" setContextVariable=""TRANSLATED_TEXT"" />
  <function.EmailConnector.LookupContactEmail input=""Martin"" setContextVariable=""CONTACT_RESULT"" />
  <function.EmailConnector.EmailTo input=""$TRANSLATED_TEXT"" recipient=""$CONTACT_RESULT""/>
</plan><!-- END -->

[AVAILABLE FUNCTIONS]

  _GLOBAL_FUNCTIONS_.BucketOutputs:
    description: When the output of a function is too big, parse the output into a number of buckets.
    inputs:
    - input: The output from a function that needs to be parse into buckets.
    - bucketCount: The number of buckets.
    - bucketLabelPrefix: The target label prefix for the resulting buckets. Result will have index appended e.g. bucketLabelPrefix='Result' => Result_1, Result_2, Result_3

  _GLOBAL_FUNCTIONS_.GetEmailAddress:
    description: Gets email address for given contact
    inputs:
    - input: the name to look up

  _GLOBAL_FUNCTIONS_.SendEmail:
    description: email the input text to a recipient
    inputs:
    - input: the text to email
    - recipient: the recipient's email address. Multiple addresses may be included if separated by ';'.

  AuthorAbility.Summarize:
    description: summarizes the input text
    inputs:
    - input: the text to summarize

  Magician.TranslateTo:
    description: translate the input to another language
    inputs:
    - input: the text to translate
    - translate_to_language: the language to translate to

[END AVAILABLE FUNCTIONS]

<goal>Summarize an input, translate to french, and e-mail to John Doe</goal>
<plan>
    <function.AuthorAbility.Summarize/>
    <function.Magician.TranslateTo translate_to_language=""French"" setContextVariable=""TRANSLATED_SUMMARY""/>
    <function._GLOBAL_FUNCTIONS_.GetEmailAddress input=""John Doe"" setContextVariable=""EMAIL_ADDRESS""/>
    <function._GLOBAL_FUNCTIONS_.SendEmail input=""$TRANSLATED_SUMMARY"" email_address=""$EMAIL_ADDRESS""/>
</plan><!-- END -->

[AVAILABLE FUNCTIONS]

  _GLOBAL_FUNCTIONS_.BucketOutputs:
    description: When the output of a function is too big, parse the output into a number of buckets.
    inputs:
    - input: The output from a function that needs to be parse into buckets.
    - bucketCount: The number of buckets.
    - bucketLabelPrefix: The target label prefix for the resulting buckets. Result will have index appended e.g. bucketLabelPrefix='Result' => Result_1, Result_2, Result_3

  _GLOBAL_FUNCTIONS_.NovelOutline :
    description: Outlines the input text as if it were a novel
    inputs:
    - input: the title of the novel to outline
    - chapterCount: the number of chapters to outline

  Emailer.EmailTo:
    description: email the input text to a recipient
    inputs:
    - input: the text to email
    - recipient: the recipient's email address. Multiple addresses may be included if separated by ';'.

  Everything.Summarize:
    description: summarize input text
    inputs:
    - input: the text to summarize

[END AVAILABLE FUNCTIONS]

<goal>Create an outline for a children's book with 3 chapters about a group of kids in a club and then summarize it.</goal>
<plan>
  <function._GLOBAL_FUNCTIONS_.NovelOutline input=""A group of kids in a club called 'The Thinking Caps' that solve mysteries and puzzles using their creativity and logic."" chapterCount=""3"" />
  <function.Everything.Summarize/>
</plan><!-- END -->

[END EXAMPLES]

[AVAILABLE FUNCTIONS]

{{$available_functions}}

[END AVAILABLE FUNCTIONS]

<goal>{{$input}}</goal>
";
}
