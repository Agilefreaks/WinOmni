<xsl:stylesheet version="1.0" xmlns:wi="http://schemas.microsoft.com/wix/2006/wi" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
  <xsl:output indent="yes"/>
  <xsl:strip-space elements="*"/>

  <xsl:template match="@*|node()" name="identity">
    <xsl:copy>
      <xsl:apply-templates select="@*|node()"/>
    </xsl:copy>
  </xsl:template>

  <!--<!–Set up keys for ignoring various file types–>-->
  <xsl:key name="filterKey" match="wi:Component[contains(wi:File/@Source, '.pdb') or contains(wi:File/@Source, '.xml')]" use="@Id"/>
  <!--<!–Match and ignore .pdb files–>-->
  <xsl:template match="wi:Component[key('filterKey', @Id)]"/>
  <xsl:template match="wi:ComponentRef[key('filterKey', @Id)]"/>

</xsl:stylesheet>