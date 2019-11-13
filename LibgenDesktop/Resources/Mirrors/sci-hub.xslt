<?xml version="1.0" encoding="utf-8"?>
<xsl:transform version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
  <xsl:template match="/">
    <xsl:for-each select="(//iframe[@id='pdf'])[1]">
      <xsl:value-of select="@src" />
    </xsl:for-each>
  </xsl:template>
</xsl:transform>
